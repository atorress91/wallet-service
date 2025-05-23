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
        IServiceScopeFactory scopeFactory,RedisCache redisCache) : base(mapper)
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
            var goal = qualification.QualificationCount >= 1      // 2.º ciclo en adelante
                ? matrixConfigResponse.Data.RangeMax
                : matrixConfigResponse.Data.Threshold;
            // Calcular porcentaje de progreso
            return Math.Min(100, (double)((totalProgressSinceLastCut / goal) * 100));
        }
        catch
        {
            return 0;
        }
    }
    private async Task VerifyRecipientQualificationsAsync(HashSet<int> userIds, int depth, IReadOnlyList<MatrixConfiguration> allMatrices)
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
                    var (anyQualified, qualifiedMatrixTypes) = await ProcessAllMatrixQualificationsAsync(recipientId);
                    if (anyQualified)
                    {
                        // Obtener los nuevos usuarios que recibieron comisiones
                        var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(recipientId, currentMatrixType);
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
                _logger.LogInformation($"Fixed qualification record ID {record.QualificationId} for user {record.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error fixing qualification record ID {record.QualificationId} for user {record.UserId}: {ex.Message}");
            }
        }

        _logger.LogInformation($"Process completed. Corrected {correctedCount} of {matrixQualifications.Count()} records.");
    }
    private async Task<int> GetNextUnqualifiedMatrixTypeAsync(int userId, IReadOnlyList<MatrixConfiguration> allMatrices)
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
    private async Task<UserProcessingResult> ProcessSingleUserAsync(int userId, List<MatrixConfiguration> allMatrices) // ← se aprovecha la cache
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
            var (qualified, matrixTypes) = await ProcessAllMatrixQualificationsAsync(userId, allMatrices); // ← sobrecarga optimizada
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
    private async Task<(bool anyQualified, List<int> qualifiedMatrixTypes)> ProcessAllMatrixQualificationsAsync(int userId, List<MatrixConfiguration> allMatrices)
    {
        var qualifiedMatrixTypes = new List<int>();
        var anyQualified = false;
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        if (allMatrices.Count == 0)
            return (false, qualifiedMatrixTypes);

        // 1) --------------------------------------------------------------------
        // Datos financieros
        // -----------------------------------------------------------------------
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId);
        var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId);
        var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(userId, brandId);
        var amountRequests = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(userId, brandId);
        availableBalance -= amountRequests;

        // 2) --------------------------------------------------------------------
        // Cargar/crear todas las calificaciones del usuario
        // -----------------------------------------------------------------------
        var userQualifications = (await _matrixQualificationRepository.GetAllByUserIdAsync(userId))
            .ToDictionary(q => q.MatrixType);

        foreach (var matrix in allMatrices)
        {
            if (!userQualifications.TryGetValue(matrix.MatrixType, out var q))
            {
                q = new MatrixQualification
                {
                    UserId = userId,
                    MatrixType = matrix.MatrixType,
                    TotalEarnings = commissions ?? 0m,
                    WithdrawnAmount = totalWithdrawn ?? 0m,
                    AvailableBalance = availableBalance,
                    IsQualified = false,
                    QualificationCount = 0,
                    LastQualificationTotalEarnings = 0m,
                    LastQualificationWithdrawnAmount = 0m,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                await _matrixQualificationRepository.CreateAsync(q);
                userQualifications[matrix.MatrixType] = q;
            }
            else
            {
                q.TotalEarnings = commissions ?? 0m;
                q.WithdrawnAmount = totalWithdrawn ?? 0m;
                q.AvailableBalance = availableBalance;
                q.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(q);
            }
        }

        // 3) --------------------------------------------------------------------
        // Procesar matriz por matriz
        // -----------------------------------------------------------------------
        var minCycle = userQualifications.Values.Min(q => q.QualificationCount);
        var usersToVerify = new HashSet<int>();

        var userInfo = await _accountServiceAdapter.GetUserInfo(userId, brandId);
        var userName = userInfo?.UserName ?? "Usuario";

        foreach (var matrixConfig in allMatrices)
        {
            var qualification = userQualifications[matrixConfig.MatrixType];

            if (qualification.QualificationCount > minCycle)
                continue;

            var qualifies = await CheckQualificationAsync(userId, matrixConfig.MatrixType);
            if (!qualifies) continue;

            // 3.a Débito
            await _walletRepository.CreateAsync(new Wallet
            {
                AffiliateId = userId,
                UserId = 1,
                Concept = $"Activación automática en {matrixConfig.MatrixName}",
                Debit = matrixConfig.FeeAmount,
                Detail = $"Ciclo de activación: {qualification.QualificationCount + 1}",
                Credit = 0,
                AffiliateUserName = userName,
                AdminUserName = "adminrecycoin",
                Status = true,
                ConceptType = "purchasing_pool",
                BrandId = brandId,
                Date = DateTime.Now,
            });

            availableBalance -= matrixConfig.FeeAmount;
            qualification.AvailableBalance = availableBalance;
            await _redisCache.InvalidateBalanceAsync(userId);
            
            // 3.b Actualizar calificación
            qualification.QualificationCount += 1;
            qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
            qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
            qualification.LastQualificationDate = DateTime.Now;
            qualification.UpdatedAt = DateTime.Now;
            await _matrixQualificationRepository.UpdateAsync(qualification);

            // 3.c Colocar usuario en la matriz
            var req = new MatrixRequest { UserId = userId, MatrixType = matrixConfig.MatrixType };
            await _accountServiceAdapter.PlaceUserInMatrix(req, brandId);

            // 3.d Comisiones
            var recipients = await ProcessMatrixCommissionsAsync(userId, matrixConfig.MatrixType, qualification.QualificationCount);
            usersToVerify.UnionWith(recipients);

            qualifiedMatrixTypes.Add(matrixConfig.MatrixType);
            anyQualified = true;
        }

        // 4) --------------------------------------------------------------------
        // Sincronizar saldo disponible en todos los registros
        // -----------------------------------------------------------------------
        foreach (var qual in userQualifications.Values)
        {
            if (qual.AvailableBalance != availableBalance)
            {
                qual.AvailableBalance = availableBalance;
                qual.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(qual);
            }
        }

        // 5) --------------------------------------------------------------------
        // Verificar calificaciones en upline receptores
        // -----------------------------------------------------------------------
        await VerifyRecipientQualificationsAsync(usersToVerify, depth: 9, allMatrices);

        return (anyQualified, qualifiedMatrixTypes);
    }
    private async Task<HashSet<int>> ProcessMatrixCommissionsAsync(int userId, int matrixType, int userQualificationCount)
    {
        var usersReceivedCommissions = new HashSet<int>();
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        await using var transaction = await _matrixEarningsRepository.BeginTransactionAsync();

        try
        {
            var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(brandId, matrixType);
            var matrixConfig = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new ApplicationException(
                    $"Error deserialize matrix configuration: {matrixConfigResponse.StatusCode}");

            // Verificar que el usuario tenga una posición válida en esta matriz
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(new MatrixRequest { UserId = userId, MatrixType = matrixType }, brandId);
            
            // Deserializamos la respuesta completa
            var matrixPositionResponse = JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!);
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
                        await _redisCache.InvalidateBalanceAsync(userId,(int)uplinePosition.UserId);
                        
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
        // 1. Obtener configuración de la matriz
        var matrixConfig = await _configurationAdapter.GetMatrixConfiguration(brandId, matrixType);

        if (matrixConfig.Content == null || matrixConfig.StatusCode != HttpStatusCode.OK)
        {
            throw new ApplicationException($"Error retrieving matrix configuration: {matrixConfig.StatusCode}");
        }

        var matrixConfigResponse = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfig.Content);

        // 2. Obtener o crear el registro de calificación
        var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

        if (qualification == null)
        {
            // Crear nuevo registro de calificación si no existe
            qualification = new MatrixQualification
            {
                UserId = userId,
                MatrixType = matrixType,
                TotalEarnings = 0,
                WithdrawnAmount = 0,
                AvailableBalance = 0,
                IsQualified = false,
                QualificationCount = 0,
                LastQualificationTotalEarnings = 0,
                LastQualificationWithdrawnAmount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _matrixQualificationRepository.CreateAsync(qualification);
            return false;
        }

        // 3. Obtener datos financieros actualizados
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId);
        var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId);

        // 4. Actualizar montos totales en el registro (usando commissions como fuente principal)
        qualification.TotalEarnings = commissions ?? 0m;
        qualification.WithdrawnAmount = totalWithdrawn ?? 0m;
        qualification.UpdatedAt = DateTime.Now;

        if (qualification is { QualificationCount: > 0, LastQualificationTotalEarnings: 0, LastQualificationWithdrawnAmount: 0 })
        {
            // Corregir valores de corte usando los valores actuales 
            // Esto asume que el progreso actual debe empezar desde cero
            qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
            qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
            qualification.LastQualificationDate = DateTime.Now;

            // Guardar para evitar sobre-calificación
            await _matrixQualificationRepository.UpdateAsync(qualification);

            // Devolver falso para evitar que se vuelva a calificar inmediatamente
            return false;
        }

        // 5. Calcular progreso desde el último corte de calificación
        var totalProgressSinceLastCut = (commissions ?? 0);
        
        var requiredAmount = qualification.QualificationCount >= 1
            ? matrixConfigResponse?.Data?.RangeMax : matrixConfigResponse?.Data?.Threshold;

        // 6. Verificar si alcanza el umbral considerando solo lo acumulado desde el último corte
        var qualifies = totalProgressSinceLastCut >= requiredAmount;

        // 7. Actualizar estado de calificación si califica y no estaba calificado
        // IMPORTANTE: No actualizamos los valores de corte (last_qualification_X) aquí
        if (qualifies && !qualification.IsQualified)
        {
            qualification.IsQualified = true;
        }

        // 8. Siempre guardar los datos financieros actualizados
        await _matrixQualificationRepository.UpdateAsync(qualification);

        // 9. Retornar si califica o no, independientemente del estado actual de IsQualified
        return qualifies;
    }
    public async Task<(bool anyQualified, List<int> qualifiedMatrixTypes)> ProcessAllMatrixQualificationsAsync(int userId)
    {
        var qualifiedMatrixTypes = new List<int>();
        var anyQualified = false;
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;
        // ---------------------------------------------------------------------
        // 1) Configuraciones de matrices (ordenadas)
        // ---------------------------------------------------------------------
        var allMatrixConfigs = await _configurationAdapter.GetAllMatrixConfigurations(brandId);
        var jsonObject = JObject.Parse(allMatrixConfigs.Content!);
        var allMatrices = jsonObject["data"]?.ToObject<List<MatrixConfiguration>>()!
            .OrderBy(m => m.MatrixType).ToList();

        if (allMatrices == null || allMatrices.Count == 0)
            return (false, qualifiedMatrixTypes);

        // ---------------------------------------------------------------------
        // 2) Datos financieros del usuario (una sola vez)
        // ---------------------------------------------------------------------
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId);
        var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId);
        var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(userId, brandId);
        var amountRequests = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(userId, brandId);
        availableBalance -= amountRequests;

        // ---------------------------------------------------------------------
        // 3) Calificaciones del usuario (crear/actualizar todas)
        // ---------------------------------------------------------------------
        var userQualifications = (await _matrixQualificationRepository.GetAllByUserIdAsync(userId))
            .ToDictionary(q => q.MatrixType);

        foreach (var matrix in allMatrices)
        {
            if (!userQualifications.TryGetValue(matrix.MatrixType, out var q))
            {
                q = new MatrixQualification
                {
                    UserId = userId,
                    MatrixType = matrix.MatrixType,
                    TotalEarnings = commissions ?? 0m,
                    WithdrawnAmount = totalWithdrawn ?? 0m,
                    AvailableBalance = availableBalance,
                    IsQualified = false,
                    QualificationCount = 0,
                    LastQualificationTotalEarnings = 0m,
                    LastQualificationWithdrawnAmount = 0m,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastQualificationDate = DateTime.Now
                };
                await _matrixQualificationRepository.CreateAsync(q);
                userQualifications[matrix.MatrixType] = q;
            }
            else
            {
                q.TotalEarnings = commissions ?? 0m;
                q.WithdrawnAmount = totalWithdrawn ?? 0m;
                q.AvailableBalance = availableBalance;
                q.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(q);
            }
        }

        // ---------------------------------------------------------------------
        // 4) Calcular el ciclo mínimo (QC) actual
        // ---------------------------------------------------------------------
        var minCycle = userQualifications.Values.Min(q => q.QualificationCount);

        // Lista para rastrear a quiénes se pagó comisión
        var usersToVerify = new HashSet<int>();

        // Nombre del usuario para la transacción
        var userInfo = await _accountServiceAdapter.GetUserInfo(userId, brandId);
        var userName = userInfo?.UserName ?? "Usuario";

        // ---------------------------------------------------------------------
        // 5) Procesar matrices SECÚENCIALMENTE
        // ---------------------------------------------------------------------
        foreach (var matrixConfig in allMatrices)
        {
            var qualification = userQualifications[matrixConfig.MatrixType];

            // ------ NUEVA REGLA DE SECUENCIA ----------------------------------
            // • Solo intentar cobrar si esta matriz aún está en el ciclo mínimo.
            // • Si ya está calificada o su QC > minCycle, se salta.
            if (qualification.QualificationCount > minCycle)
                continue;
            // ------------------------------------------------------------------

            // ¿Alcanza el umbral?
            var qualifies = await CheckQualificationAsync(userId, matrixConfig.MatrixType);
            if (!qualifies) continue; // No llegó al threshold → pasa a la siguiente

            // -----------------------------------------------------------------
            // 5.a Débito por la tarifa
            // -----------------------------------------------------------------
            await _walletRepository.CreateAsync(new Wallet
            {
                AffiliateId = userId,
                UserId = 1,
                Concept = $"Activación automática en {matrixConfig.MatrixName}",
                Debit = matrixConfig.FeeAmount,
                Detail = $"Ciclo de activación: {qualification.QualificationCount + 1}",
                Credit = 0,
                AffiliateUserName = userName,
                AdminUserName = "adminrecycoin",
                Status = true,
                ConceptType = "purchasing_pool",
                BrandId = brandId,
                Date = DateTime.Now
            });

            availableBalance -= matrixConfig.FeeAmount;
            qualification.AvailableBalance = availableBalance;

            // -----------------------------------------------------------------
            // 5.b Actualizar la calificación
            // -----------------------------------------------------------------
            qualification.QualificationCount += 1;
            qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
            qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
            qualification.LastQualificationDate = DateTime.Now;
            qualification.UpdatedAt = DateTime.Now;
            await _matrixQualificationRepository.UpdateAsync(qualification);

            // -----------------------------------------------------------------
            // 5.c Colocar al usuario en la matriz
            // -----------------------------------------------------------------
            var request = new MatrixRequest { UserId = userId, MatrixType = matrixConfig.MatrixType };
            await _accountServiceAdapter.PlaceUserInMatrix(request, brandId);

            // -----------------------------------------------------------------
            // 5.d Procesar comisiones
            // -----------------------------------------------------------------
            var recipientUsers = await ProcessMatrixCommissionsAsync(userId, matrixConfig.MatrixType, qualification.QualificationCount);
            usersToVerify.UnionWith(recipientUsers);
            await _redisCache.InvalidateBalanceAsync(userId);
            await _redisCache.InvalidateBalanceAsync(recipientUsers.ToArray());
            
            qualifiedMatrixTypes.Add(matrixConfig.MatrixType);
            anyQualified = true;
        }

        // ---------------------------------------------------------------------
        // 6) Sincronizar saldo disponible en todos los registros
        // ---------------------------------------------------------------------
        foreach (var qual in userQualifications.Values)
        {
            if (qual.AvailableBalance != availableBalance)
            {
                qual.AvailableBalance = availableBalance;
                qual.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(qual);
            }
        }

        // ---------------------------------------------------------------------
        // 7) Verificar calificaciones en quienes recibieron comisión
        // ---------------------------------------------------------------------
        await VerifyRecipientQualificationsAsync(usersToVerify, 1, allMatrices);

        return (anyQualified, qualifiedMatrixTypes);
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
            var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);
            var matrixConfig = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new ApplicationException(
                    $"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new ApplicationException("Matrix configuration data is missing or invalid");

            // 2. Verificar si el usuario ya tiene una posición en esta matriz
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(new MatrixRequest { UserId = userId, MatrixType = matrixType }, _brandService.BrandId);
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

            var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(request.UserId, _brandService.BrandId);
            var pendingAmount = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(request.UserId, _brandService.BrandId);
            availableBalance -= pendingAmount;
            if (availableBalance < matrixConfig.FeeAmount)
                return false;

            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(new MatrixRequest { UserId = targetUserId, MatrixType = request.MatrixType }, _brandService.BrandId);
            
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
            
            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(targetUserId, request.MatrixType);
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
        var usersToProcess = userIds?.Length > 0 ? userIds 
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