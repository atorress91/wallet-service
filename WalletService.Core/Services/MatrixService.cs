using System.Collections.Concurrent;
using System.Net;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WalletService.Core.Caching;
using WalletService.Core.Caching.Extensions;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Requests.MatrixRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services;

public class MatrixService : BaseService, IMatrixService
{
    private readonly IConfigurationAdapter _configurationAdapter;
    private readonly IBrandService _brandService;
    private readonly IMatrixQualificationRepository _matrixQualificationRepository;
    private readonly IMatrixEarningsRepository _matrixEarningsRepository;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MatrixService> _logger;
    private readonly RedisCache _redisCache;

    public MatrixService(ILogger<MatrixService> logger,
        IMapper mapper, IConfigurationAdapter configurationAdapter, IBrandService brandService,
        IMatrixQualificationRepository matrixQualificationRepository,
        IMatrixEarningsRepository matrixEarningsRepository,
        IAccountServiceAdapter accountServiceAdapter, IWalletRepository walletRepository,
        IWalletRequestRepository walletRequestRepository,
        IServiceScopeFactory scopeFactory, RedisCache redisCache) : base(mapper)
    {
        _brandService = brandService;
        _configurationAdapter = configurationAdapter;
        _matrixQualificationRepository = matrixQualificationRepository;
        _matrixEarningsRepository = matrixEarningsRepository;
        _accountServiceAdapter = accountServiceAdapter;
        _walletRepository = walletRepository;
        _walletRequestRepository = walletRequestRepository;
        _scopeFactory = scopeFactory;

        _logger = logger;
        _redisCache = redisCache;
    }

    // -----------------------------------------------------------------------------
    // 1.  NUEVO   –  Método transaccional que centraliza débito + calificación
    // -----------------------------------------------------------------------------
    private async Task<decimal> ApplyQualificationAsync(MatrixQualification qualification, MatrixConfiguration matrixCfg,
        string userName,
        decimal availableBalance)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        await using var tx = await _matrixEarningsRepository.BeginTransactionAsync();

        // 1.a Débito en el wallet
        await _walletRepository.CreateAsync(new Wallet
        {
            AffiliateId = qualification.UserId,
            UserId = 1,
            Concept = $"Activación automática en {matrixCfg.MatrixName}",
            Detail = $"Ciclo de activación: {qualification.QualificationCount + 1}",
            Debit = matrixCfg.FeeAmount,
            Credit = 0,
            AffiliateUserName = userName,
            AdminUserName = "adminrecycoin",
            Status = true,
            ConceptType = "purchasing_pool",
            BrandId = brandId,
            Date = DateTime.Now,
        });

        // 1.b Actualizar saldo disponible en memoria
        availableBalance -= matrixCfg.FeeAmount;

        // 1.c Actualizar calificación
        qualification.IsQualified = true;
        qualification.QualificationCount += 1;
        qualification.AvailableBalance = availableBalance;
        qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
        qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
        qualification.LastQualificationDate = DateTime.Now;
        qualification.UpdatedAt = DateTime.Now;

        await _matrixQualificationRepository.UpdateAsync(qualification);

        await tx.CommitAsync();
        return availableBalance;
    }

    private async Task<double> GetQualificationProgressAsync(int userId, int matrixType)
    {
        try
        {
            // Obtener configuración de la matriz
            var matrixConfig = await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);
            var matrixConfigResponse =
                JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfig.Content!);

            if (matrixConfigResponse?.Data == null)
                return 0;

            // Obtener calificación
            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

            if (qualification == null)
                return 0;

            // Obtener datos financieros
            var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, _brandService.BrandId);
            var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId);

            // Calcular progreso
            var earningsSinceLastQualification = (commissions ?? 0);
            var withdrawnSinceLastQualification = (totalWithdrawn ?? 0);
            var totalProgressSinceLastCut = earningsSinceLastQualification + withdrawnSinceLastQualification;

            // ➋ Elegir meta según el ciclo
            var cycle = qualification.QualificationCount;
            var goal = cycle == 0
                ? matrixConfigResponse.Data.Threshold
                : matrixConfigResponse.Data.RangeMax * cycle;
            // Calcular porcentaje de progreso
            return Math.Min(100, (double)((totalProgressSinceLastCut / goal) * 100));
        }
        catch
        {
            return 0;
        }
    }

    private async Task VerifyRecipientQualificationsAsync(HashSet<int> userIds, int depth,
        IReadOnlyList<MatrixConfiguration> allMatrices)
    {
        await _redisCache.InvalidateBalanceAsync(userIds.ToArray());
        while (true)
        {
            // Limitación de profundidad para evitar ciclos excesivos
            if (depth > 7) return;

            // Copia de la lista para evitar modificaciones durante la iteración
            var recipientUserIds = userIds.ToList();
            var nextLevelUsers = new HashSet<int>();

            foreach (var recipientId in recipientUserIds)
            {
                // Determinar el tipo de matriz actual para este usuario
                var currentMatrixType = await GetNextUnqualifiedMatrixTypeAsync(recipientId, allMatrices);

                // Verificar calificación solo para la matriz relevante
                var qualified = await CheckQualificationAsync(recipientId, currentMatrixType);

                // Si califica, procesar calificación completa y recolectar nuevos usuarios
                if (qualified)
                {
                    var (anyQualified, qualifiedMatrixTypes) = await ProcessAllMatrixQualificationsAsync(recipientId,allMatrices.ToList());
                    if (anyQualified)
                    {
                        // Obtener los nuevos usuarios que recibieron comisiones
                        var qualification =
                            await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(recipientId,
                                currentMatrixType);
                        if (qualification != null && qualifiedMatrixTypes.Contains(currentMatrixType))
                        {
                            var newRecipientsUsers = await ProcessMatrixCommissionsAsync(recipientId, currentMatrixType,
                                qualification.QualificationCount);

                            // Agregar estos usuarios para la siguiente ronda de verificación
                            foreach (var newRecipient in newRecipientsUsers)
                            {
                                nextLevelUsers.Add(newRecipient);
                            }
                        }
                    }
                }
            }

            // Procesar el siguiente nivel de usuarios si hay alguno y no superamos el límite de profundidad
            if (nextLevelUsers.Count > 0)
            {
                userIds = nextLevelUsers;
                depth = depth + 1;
                continue;
            }

            break;
        }
    }

    public async Task FixInconsistentQualificationRecordsAsync()
    {
        // Obtener todos los registros de calificación con contadores positivos pero valores de corte en 0
        var inconsistentRecords = await _matrixQualificationRepository.GetAllInconsistentRecordsAsync();
        var correctedCount = 0;

        var matrixQualifications = inconsistentRecords as MatrixQualification[] ?? inconsistentRecords.ToArray();
        foreach (var record in matrixQualifications)
        {
            try
            {
                // 1. Obtener los datos financieros reales y actualizados del usuario
                var commissions =
                    await _walletRepository.GetQualificationBalanceAsync(record.UserId, _brandService.BrandId);
                var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(record.UserId);
                var availableBalance =
                    await _walletRepository.GetAvailableBalanceByAffiliateId((int)record.UserId, _brandService.BrandId);
                var amountRequests = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(
                    (int)record.UserId,
                    _brandService.BrandId);
                availableBalance -= amountRequests;

                // 2. Actualizar los valores financieros correctos en el registro
                record.TotalEarnings = commissions ?? 0m;
                record.WithdrawnAmount = totalWithdrawn ?? 0m;
                record.AvailableBalance = availableBalance;

                // 3. Establecer los valores de corte para que coincidan con los valores actuales
                // Esto resetea efectivamente el progreso hacia la siguiente calificación
                record.LastQualificationTotalEarnings = record.TotalEarnings;
                record.LastQualificationWithdrawnAmount = record.WithdrawnAmount;
                record.LastQualificationDate = DateTime.Now;
                record.UpdatedAt = DateTime.Now;

                // 4. Guardar los cambios
                await _matrixQualificationRepository.UpdateAsync(record);

                correctedCount++;
                _logger.LogInformation(
                    $"Fixed qualification record ID {record.QualificationId} for user {record.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    $"Error fixing qualification record ID {record.QualificationId} for user {record.UserId}: {ex.Message}");
            }
        }

        _logger.LogInformation(
            $"Process completed. Corrected {correctedCount} of {matrixQualifications.Count()} records.");
    }

    private async Task<int> GetNextUnqualifiedMatrixTypeAsync(int userId,
        IReadOnlyList<MatrixConfiguration> allMatrices)
    {
        var qualifications = await _matrixQualificationRepository.GetAllByUserIdAsync(userId);

        var matrixQualifications = qualifications as MatrixQualification[] ?? qualifications.ToArray();
        var minCycle = matrixQualifications.Length != 0 ? matrixQualifications.Min(q => q.QualificationCount) : 0;

        // buscamos la PRIMERA matriz cuyo QC == minCycle  y   !IsQualified
        foreach (var m in allMatrices.OrderBy(m => m.MatrixType))
        {
            var qualification = matrixQualifications.FirstOrDefault(x => x.MatrixType == m.MatrixType);

            var faltante = qualification == null // nunca se creó
                           || !qualification.IsQualified // aún no alcanzó el umbral
                           || qualification.QualificationCount < minCycle;

            if (faltante) return m.MatrixType;
        }

        // Si todas están calificadas con el mismo ciclo,
        // el próximo objetivo es la Matrix 1 del ciclo siguiente.
        return 1;
    }

    private async Task<UserProcessingResult> ProcessSingleUserAsync(int userId, 
        List<MatrixConfiguration> allMatrices) 
    {
        var userResult = new UserProcessingResult
        {
            UserId = userId,
            ProcessedTime = DateTime.Now,
            MatricesQualified = new List<MatrixQualificationInfo>()
        };

        try
        {
            // 1. Procesar calificaciones usando la lista cacheada
            var (qualified, matrixTypes) =
                await ProcessAllMatrixQualificationsAsync(userId, allMatrices); // ← sobrecarga optimizada
            userResult.WasQualified = qualified;

            if (qualified && matrixTypes.Any())
            {
                // 2. Obtener las calificaciones concretas del usuario
                var qualifications = await _matrixQualificationRepository.GetAllByUserIdAsync(userId);

                foreach (var q in qualifications.Where(q => matrixTypes.Contains(q.MatrixType)))
                {
                    // Encontrar el nombre desde la config ya recibida
                    var matrixName = allMatrices
                        .First(m => m.MatrixType == q.MatrixType)
                        .MatrixName;

                    userResult.MatricesQualified.Add(new MatrixQualificationInfo
                    {
                        MatrixType = q.MatrixType,
                        MatrixName = matrixName,
                        QualificationCount = q.QualificationCount,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            userResult.ErrorMessage = ex.Message;
        }

        return userResult;
    }

    private async Task<(bool anyQualified, List<int> qualifiedMatrixTypes)> ProcessAllMatrixQualificationsAsync(int userId,
        List<MatrixConfiguration> allMatrices)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        var anyQualified = false;
        var qualifiedMatrixTypes = new List<int>();
        var usersToVerify = new HashSet<int>();

        // --- Datos financieros base ------------------------------------------------
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId) ?? 0m;
        var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId) ?? 0m;
        var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(userId, brandId);
        var pendingRequests = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(userId, brandId);
        availableBalance -= pendingRequests;

        // --- Cargar/crear calificaciones en memoria --------------------------------
        var qualifications = (await _matrixQualificationRepository.GetAllByUserIdAsync(userId))
            .ToDictionary(q => q.MatrixType);

        foreach (var m in allMatrices)
        {
            if (!qualifications.TryGetValue(m.MatrixType, out var q))
            {
                q = new MatrixQualification
                {
                    UserId = userId,
                    MatrixType = m.MatrixType,
                    TotalEarnings = commissions,
                    WithdrawnAmount = totalWithdrawn,
                    AvailableBalance = availableBalance,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _matrixQualificationRepository.CreateAsync(q);
                qualifications[m.MatrixType] = q;
            }
            else
            {
                // Sincronizar saldo para que el progreso sea correcto
                q.TotalEarnings = commissions;
                q.WithdrawnAmount = totalWithdrawn;
                q.AvailableBalance = availableBalance;
                q.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(q);
            }
        }

        // --- Nombre del usuario (para el débito) -----------------------------------
        var userName = (await _accountServiceAdapter.GetUserInfo(userId, brandId))?.UserName ?? "Usuario";

        // --- Procesar cada matriz secuencialmente ----------------------------------
        foreach (var m in allMatrices)
        {
            var q = qualifications[m.MatrixType];

            // Si ya está en un ciclo más avanzado que otras, saltar (regla de secuencia)
            var minCycle = qualifications.Values.Min(x => x.QualificationCount);
            if (q.QualificationCount > minCycle) continue;

            // ¿Cumple el umbral?
            var qualifies = await CheckQualificationAsync(userId, m.MatrixType);
            if (!qualifies) continue;

            // Aplicar calificación (débito + actualización) **dentro de una transacción**
            availableBalance = await ApplyQualificationAsync(q, m, userName, availableBalance);

            qualifiedMatrixTypes.Add(m.MatrixType);
            anyQualified = true;

            // Colocar al usuario en la matriz y procesar comisiones
            await _accountServiceAdapter.PlaceUserInMatrix(new MatrixRequest { UserId = userId, MatrixType = m.MatrixType }, brandId);

            var recipients = await ProcessMatrixCommissionsAsync(userId, m.MatrixType, q.QualificationCount);
            usersToVerify.UnionWith(recipients);

            await _redisCache.InvalidateBalanceAsync(userId);
            await _redisCache.InvalidateBalanceAsync(recipients.ToArray());
        }

        // --- Sincronizar saldo final en todos los registros ------------------------
        foreach (var qual in qualifications.Values)
            if (qual.AvailableBalance != availableBalance)
            {
                qual.AvailableBalance = availableBalance;
                qual.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(qual);
            }
        
        if (usersToVerify.Count > 0)
        {
            await VerifyRecipientQualificationsAsync(usersToVerify, 1, allMatrices);
        }

        return (anyQualified, qualifiedMatrixTypes);
    }

    private async Task<HashSet<int>> ProcessMatrixCommissionsAsync(int userId, int matrixType,
        int userQualificationCount)
    {
        var usersReceivedCommissions = new HashSet<int>();
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        await using var transaction = await _matrixEarningsRepository.BeginTransactionAsync();

        try
        {
            var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(brandId, matrixType);
            var matrixConfig =
                JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new ApplicationException(
                    $"Error deserialize matrix configuration: {matrixConfigResponse.StatusCode}");

            // Verificar que el usuario tenga una posición válida en esta matriz
            var positionResponse =
                await _accountServiceAdapter.IsActiveInMatrix(
                    new MatrixRequest { UserId = userId, MatrixType = matrixType }, brandId);

            // Deserializamos la respuesta completa
            var matrixPositionResponse =
                JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!);
            // Extraemos la posición desde la propiedad Data
            var position = matrixPositionResponse?.Data;

            if (positionResponse.StatusCode != HttpStatusCode.OK || position == false)
            {
                throw new ApplicationException($"User {userId} does not have a valid position in matrix {matrixType}");
            }

            // Calcular el 10% de comisión del monto de la tarifa
            var commissionAmount = matrixConfig.Data.FeeAmount * 0.1m;

            // Obtener todas las posiciones superiores (upline)
            var uplinePositionsResponse = await _accountServiceAdapter.GetUplinePositionsAsync(
                new MatrixRequest { UserId = userId, MatrixType = matrixType }, brandId);

            var jObject = JObject.Parse(uplinePositionsResponse.Content!);
            var allUplinePositions = jObject["data"]?.ToObject<IEnumerable<MatrixPositionDto>>();

            if (uplinePositionsResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error retrieving upline positions: {uplinePositionsResponse.StatusCode}");

            if (allUplinePositions != null)
            {
                // Filtrar posiciones del mismo tipo de matriz y usuarios únicos
                var filteredPositions = allUplinePositions
                    .Where(p => p.MatrixType == matrixType)
                    .GroupBy(p => p.UserId)
                    .Select(g => g.OrderBy(p => p.Level).First()) // Tomar la posición más cercana para cada usuario
                    .OrderBy(p => p.Level)
                    .Take(matrixConfig.Data.Levels)
                    .ToList();

                // Guardar para log o depuración
                _logger.LogInformation($"Processing commissions for user {userId} in matrix {matrixType}. " +
                                       $"Found {filteredPositions.Count} eligible upline positions.");

                foreach (var uplinePosition in filteredPositions)
                {
                    // Verificar si el upline está calificado en esta matriz y tiene al menos el mismo número de calificaciones
                    var uplineQualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(
                        uplinePosition.UserId, matrixType);

                    // Solo pagar si está calificado Y tiene al menos el mismo contador de calificaciones
                    if (uplineQualification is { IsQualified: true } &&
                        uplineQualification.QualificationCount >= userQualificationCount)
                    {
                        // Crear registro de ganancia para el upline calificado
                        var earning = new MatrixEarning
                        {
                            UserId = uplinePosition.UserId,
                            MatrixType = matrixType,
                            Amount = commissionAmount,
                            SourceUserId = userId,
                            EarningType = "Matrix_Qualification",
                            CreatedAt = DateTime.Now
                        };

                        // Pasar el contador de calificaciones al crear la ganancia
                        await _matrixEarningsRepository.CreateAsync(earning, userQualificationCount);
                        await _redisCache.InvalidateBalanceAsync(userId, (int)uplinePosition.UserId);

                        // Agregar este usuario a la lista de los que recibieron comisiones
                        usersReceivedCommissions.Add((int)uplinePosition.UserId);
                    }
                }
            }

            // Si todo va bien, confirmar la transacción
            await transaction.CommitAsync();
            return usersReceivedCommissions;
        }
        catch (Exception ex)
        {
            // En caso de error, revertir la transacción
            await transaction.RollbackAsync();
            _logger.LogWarning($"Error en ProcessMatrixCommissionsAsync: {ex.Message}");
            throw; // Relanzar la excepción para manejo superior
        }
    }

    public async Task<bool> CheckQualificationAsync(long userId, int matrixType)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        // 2.a Configuración de la matriz
        var cfgResp = await _configurationAdapter.GetMatrixConfiguration(brandId, matrixType);
        if (cfgResp.Content == null || cfgResp.StatusCode != HttpStatusCode.OK)
            throw new ApplicationException($"Error retrieving matrix configuration: {cfgResp.StatusCode}");

        var cfg = JsonConvert
            .DeserializeObject<MatrixConfigurationResponse>(cfgResp.Content!)!
            .Data;

        // 2.b Cargar (o crear) registro de calificación
        var qual = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);
        if (qual == null)
        {
            qual = new MatrixQualification
            {
                UserId = userId,
                MatrixType = matrixType,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _matrixQualificationRepository.CreateAsync(qual);
            // No puede calificar recién creado
            return false;
        }

        // 2.c Refrescar montos financieros
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId) ?? 0m;
        var withdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId) ?? 0m;
        qual.TotalEarnings = commissions;
        qual.WithdrawnAmount = withdrawn;
        qual.UpdatedAt = DateTime.Now;
        await _matrixQualificationRepository.UpdateAsync(qual);

        // 2.d Progreso desde el último corte
        var cycle = qual.QualificationCount; // 0 → primer ciclo
        var requiredAmount = cycle == 0
            ? cfg!.Threshold
            : cfg!.RangeMax * cycle;

        var progress = commissions - qual.LastQualificationTotalEarnings
            + withdrawn - qual.LastQualificationWithdrawnAmount;

        return progress >= requiredAmount; // ***sin tocar IsQualified aquí*** :contentReference[oaicite:0]{index=0}
    }

    public async Task<(bool anyQualified, List<int> qualifiedMatrixTypes)> ProcessAllMatrixQualificationsAsync(int userId)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        var allMatrices = await _redisCache.Remember(
            $"matrix_cfg_{brandId}",
            TimeSpan.FromMinutes(10),
            async () =>
            {
                var resp = await _configurationAdapter.GetAllMatrixConfigurations(brandId);
                var data = JObject.Parse(resp.Content!)["data"]!.ToObject<List<MatrixConfiguration>>()!;
                return data.OrderBy(m => m.MatrixType).ToList();
            });

        return await ProcessAllMatrixQualificationsAsync(userId, allMatrices);
    }

    public async Task<bool> WithdrawFromMatrixAsync(int userId, short matrixType, decimal amount)
    {
        try
        {
            // Obtener configuración de matriz
            var matrixConfigResponse =
                await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);
            var matrixConfig =
                JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            // Verificar límites de retiro
            if (amount < matrixConfig!.Data!.MinWithdraw || amount > matrixConfig.Data.MaxWithdraw)
                return false;

            // Obtener registro de calificación del usuario
            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);
            if (qualification == null || qualification.AvailableBalance < amount)
                return false;

            // Actualizar registro de calificación
            qualification.AvailableBalance -= amount;
            qualification.WithdrawnAmount += amount;

            return await _matrixQualificationRepository.UpdateAsync(qualification);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error withdrawing from matrix: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessAdminMatrixPlacementAsync(int userId, int matrixType)
    {
        try
        {
            // 1. Obtener configuración de la matriz específica solicitada
            var matrixConfigResponse =
                await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);
            var matrixConfig =
                JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new ApplicationException("Matrix configuration data is missing or invalid");

            // 2. Verificar si el usuario ya tiene una posición en esta matriz
            var positionResponse =
                await _accountServiceAdapter.IsActiveInMatrix(
                    new MatrixRequest { UserId = userId, MatrixType = matrixType }, _brandService.BrandId);
            var existing = JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!);

            if (positionResponse.IsSuccessful && existing?.Data == true)
                // Si el usuario ya tiene una posición activa en la matriz, no se puede activar nuevamente
                return false;

            // 4. Obtener o crear el registro de calificación para esta matriz específica
            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

            if (qualification is { IsQualified: true })
                return false;

            if (qualification == null)
            {
                qualification = new MatrixQualification
                {
                    UserId = userId,
                    MatrixType = matrixType,
                    TotalEarnings = matrixConfig.Data.Threshold, // Establecer como si hubiera alcanzado el umbral
                    WithdrawnAmount = 0,
                    AvailableBalance = 0,
                    IsQualified = true,
                    QualificationCount = 1,
                    LastQualificationTotalEarnings = matrixConfig.Data.Threshold,
                    LastQualificationWithdrawnAmount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastQualificationDate = DateTime.Now
                };

                await _matrixQualificationRepository.CreateAsync(qualification);
            }
            else
            {
                // Actualizar el registro existente para marcarlo como calificado
                qualification.IsQualified = true;
                qualification.QualificationCount += 1;
                qualification.UpdatedAt = DateTime.Now;

                await _matrixQualificationRepository.UpdateAsync(qualification);
            }

            // 5. Procesar comisiones para los líderes
            await ProcessMatrixCommissionsAsync(userId, matrixType, qualification.QualificationCount);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error in admin matrix placement: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessDirectPaymentMatrixActivationAsync(MatrixRequest request)
    {
        try
        {
            var targetUserId = request.RecipientId ?? request.UserId;

            var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(
                _brandService.BrandId, request.MatrixType);
            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error al obtener configuración de matriz: {matrixConfigResponse.StatusCode}");

            var matrixConfig = JsonConvert
                .DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!)?
                .Data;
            if (matrixConfig == null)
                throw new ApplicationException("La configuración de la matriz es inválida");

            var availableBalance =
                await _walletRepository.GetAvailableBalanceByAffiliateId(request.UserId, _brandService.BrandId);
            var pendingAmount =
                await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(request.UserId,
                    _brandService.BrandId);
            availableBalance -= pendingAmount;
            if (availableBalance < matrixConfig.FeeAmount)
                return false;

            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(
                new MatrixRequest { UserId = targetUserId, MatrixType = request.MatrixType }, _brandService.BrandId);

            var existing = JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!)?.Data;

            if (positionResponse.IsSuccessful && existing == true)
                // Si el usuario ya tiene una posición activa en la matriz, no se puede activar nuevamente
                return false;

            var payerInfo = await _accountServiceAdapter.GetUserInfo(request.UserId, _brandService.BrandId);
            var payerName = payerInfo?.UserName ?? "Usuario";

            var targetInfo = await _accountServiceAdapter.GetUserInfo(targetUserId, _brandService.BrandId);
            var targetName = targetInfo?.UserName ?? "Usuario";

            var debitRequest = new Wallet()
            {
                AffiliateId = request.UserId,
                UserId = 0,
                Concept = $"Activación de {matrixConfig.MatrixName} para {targetName}",
                Deferred = 0,
                Debit = matrixConfig.FeeAmount,
                Status = true,
                AffiliateUserName = payerName,
                AdminUserName = "adminrecycoin",
                ConceptType = "purchasing_pool",
                BrandId = _brandService.BrandId,
                Date = DateTime.Now,
            };

            var qualification =
                await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(targetUserId, request.MatrixType);
            if (qualification?.IsQualified == true)
                return false;

            var debitResult = await _walletRepository.CreateWalletAsync(debitRequest);
            if (debitResult == null)
                throw new ApplicationException("No se pudo procesar el débito");

            if (qualification == null)
            {
                qualification = new MatrixQualification
                {
                    UserId = targetUserId,
                    MatrixType = request.MatrixType,
                    TotalEarnings = matrixConfig.Threshold,
                    WithdrawnAmount = 0,
                    AvailableBalance = availableBalance - matrixConfig.FeeAmount,
                    IsQualified = true,
                    QualificationCount = 1,
                    LastQualificationTotalEarnings = matrixConfig.Threshold,
                    LastQualificationWithdrawnAmount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastQualificationDate = DateTime.Now
                };
                await _matrixQualificationRepository.CreateAsync(qualification);
                await _redisCache.InvalidateBalanceAsync(targetUserId);
            }
            else
            {
                qualification.IsQualified = true;
                qualification.QualificationCount += 1;
                qualification.AvailableBalance = availableBalance - matrixConfig.FeeAmount;
                qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
                qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
                qualification.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(qualification);
            }

            await ProcessMatrixCommissionsAsync(targetUserId, request.MatrixType, qualification.QualificationCount);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<BatchProcessingResult> ProcessAllUsersMatrixQualificationsAsync(int[]? userIds = null)
    {
        _logger.LogInformation("Starting matrix qualifications processing. IDs: {Count}", userIds?.Length ?? 0);
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        var result = new BatchProcessingResult
        {
            StartTime = DateTime.Now,
            ProcessedUsers = new ConcurrentBag<UserProcessingResult>()
        };

        // 1. ---------------- Cachear configuraciones ----------------------------
        var allMatrices = await _redisCache.Remember(
            $"matrix_cfg_{brandId}",
            TimeSpan.FromMinutes(10),
            async () =>
            {
                var resp = await _configurationAdapter.GetAllMatrixConfigurations(brandId);
                var data = JObject.Parse(resp.Content!)["data"]!.ToObject<List<MatrixConfiguration>>()!;
                return data.OrderBy(m => m.MatrixType).ToList();
            });

        // 2. ---------------- Obtener usuarios a procesar ------------------------
        var usersToProcess = userIds?.Length > 0
            ? userIds
            : (await _walletRepository.GetUserIdsWithCommissionsGreaterThanOrEqualTo50(brandId))
            .Where(u => u != 0)
            .Select(u => (int)u)
            .ToArray();

        // 3. ---------------- Paralelismo controlado -----------------------------
        const int maxDop = 8; // <- ajustar CPU/BD
        using var throttler = new SemaphoreSlim(maxDop);

        var tasks = usersToProcess.Select(async uid =>
        {
            await throttler.WaitAsync();
            try
            {
                using var scope = _scopeFactory.CreateScope(); // DbContext aislado
                var svc = scope.ServiceProvider.GetRequiredService<MatrixService>();
                var r = await svc.ProcessSingleUserAsync(uid, allMatrices); // ⇢ método interno refactorizado
                result.ProcessedUsers.Add(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando usuario {UserId} en ProcessSingleUserAsync", uid);
                var failedResult = new UserProcessingResult
                {
                    UserId = uid,
                    ProcessedTime = DateTime.Now,
                    WasQualified = false,
                    MatricesQualified = new List<MatrixQualificationInfo>(),
                    ErrorMessage = ex.ToString()
                };
                result.ProcessedUsers.Add(failedResult);
            }
            finally
            {
                throttler.Release();
            }
        });

        await Task.WhenAll(tasks);

        // 4. ---------------- Estadísticas finales -------------------------------
        result.TotalProcessed = result.ProcessedUsers.Count;
        result.TotalQualified = result.ProcessedUsers.Count(u => u.WasQualified);
        result.EndTime = DateTime.Now;
        result.ElapsedTimeSeconds = (result.EndTime - result.StartTime).TotalSeconds;
        return result;
    }
}