using System.Collections.Concurrent;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using WalletService.Models.Constants;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.MatrixRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

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
    private readonly ITransactionRepository _transactionRepository;
    public MatrixService(ILogger<MatrixService> logger,
        IMapper mapper, IConfigurationAdapter configurationAdapter, IBrandService brandService,
        IMatrixQualificationRepository matrixQualificationRepository,
        IMatrixEarningsRepository matrixEarningsRepository,
        IAccountServiceAdapter accountServiceAdapter, IWalletRepository walletRepository,
        IWalletRequestRepository walletRequestRepository,
        IServiceScopeFactory scopeFactory, RedisCache redisCache,
        ITransactionRepository transactionRepository) : base(mapper)
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
        _transactionRepository = transactionRepository;
    }
    private static bool IsRequestValid(IpnRequest request, IHeaderDictionary headers)
    {
        if (string.IsNullOrEmpty(request.ipn_mode) || request.ipn_mode.ToLower() != "hmac")
            return false;

        if (!headers.TryGetValue("Hmac", out var receivedHmac) || String.IsNullOrEmpty(receivedHmac))
            return false;

        if (string.IsNullOrEmpty(request.merchant))
            return false;

        if (request.ipn_type != "api")
            return false;

        var validCurrencies = new[]
        {
            "USDT.TRC20", "USDT.BEP20"
        };

        if (!validCurrencies.Contains(request.currency1))
            return false;

        return true;
    }

    private async Task<decimal> ApplyQualificationAsync(MatrixQualification qualification, MatrixConfiguration matrixCfg,
        string userName, decimal availableBalance)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        // ✅ VALIDACIÓN DE SALDO SUFICIENTE ANTES DE PROCEDER
        if (availableBalance < matrixCfg.FeeAmount)
        {
            _logger.LogWarning(
                "User {UserId} does not have enough balance to qualify in {MatrixName}. Available balance: {AvailableBalance:C}, Required: {RequiredFee:C}",
                qualification.UserId, matrixCfg.MatrixName, availableBalance, matrixCfg.FeeAmount);

            throw new InvalidOperationException($"Insufficient balance for qualification. Available: {availableBalance:C}, Required: {matrixCfg.FeeAmount:C}");
        }

        await using var tx = await _matrixEarningsRepository.BeginTransactionAsync();

        try
        {
            var adminBase = Math.Round(matrixCfg.FeeAmount * 0.30m, 2);
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
                AdminUserName = Constants.RecycoinAdmin,
                Status = true,
                ConceptType = "purchasing_pool",
                BrandId = brandId,
                Date = DateTime.Now,
            });

            // 1.1a Credito al admin en wallet
            await _walletRepository.CreateAsync(new Wallet {
                AffiliateId = 0,
                UserId = 1, // o el Id de sistema que ya usas
                Concept = $"Fee admin 30% - {matrixCfg.MatrixName} (User {qualification.UserId})",
                Detail = $"Ciclo {qualification.QualificationCount + 1}",
                Debit = 0,
                Credit = adminBase,
                AffiliateUserName = Constants.RecycoinAdmin,
                AdminUserName = Constants.RecycoinAdmin,
                Status = true,
                ConceptType = "admin_fee",
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
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            throw;
        }
    }
    private async Task VerifyRecipientQualificationsAsync(HashSet<int> userIds, int depth,
        IReadOnlyList<MatrixConfiguration> allMatrices)
    {
        await _redisCache.InvalidateBalanceAsync(userIds.ToArray());

        // Materializamos una lista una sola vez para reutilizarla en llamadas internas
        var matricesList = allMatrices as List<MatrixConfiguration> ?? allMatrices.ToList();

        // Límite de profundidad controlado desde el encabezado del bucle
        for (; depth <= 7 && userIds.Count > 0; depth++)
        {
            var nextLevelUsers = new HashSet<int>();
            var recipientsSnapshot = userIds.ToList();

            foreach (var recipientId in recipientsSnapshot)
            {
                var newRecipients = await ProcessRecipientAsync(recipientId, matricesList);
                if (newRecipients.Count > 0)
                    nextLevelUsers.UnionWith(newRecipients);
            }

            userIds = nextLevelUsers;
        }
    }

    private async Task<HashSet<int>> ProcessRecipientAsync(int recipientId, List<MatrixConfiguration> allMatrices)
    {
        var nextLevelUsers = new HashSet<int>();

        // Determinar el tipo de matriz actual
        var currentMatrixType = await GetNextUnqualifiedMatrixTypeAsync(recipientId, allMatrices);

        // Verificar calificación solo para la matriz relevante
        var qualified = await CheckQualificationAsync(recipientId, currentMatrixType);
        if (!qualified) return nextLevelUsers;

        // Procesar calificaciones de todas las matrices
        var (anyQualified, qualifiedMatrixTypes) =
            await ProcessAllMatrixQualificationsAsync(recipientId, allMatrices);
        if (!anyQualified) return nextLevelUsers;

        // Verificar que la calificación actual esté incluida
        var qualification =
            await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(recipientId, currentMatrixType);
        if (qualification == null || !qualifiedMatrixTypes.Contains(currentMatrixType))
            return nextLevelUsers;

        // Procesar comisiones y recopilar nuevos destinatarios
        var newRecipientsUsers = await ProcessMatrixCommissionsAsync(
            recipientId,
            currentMatrixType,
            qualification.QualificationCount
        );

        nextLevelUsers.UnionWith(newRecipientsUsers);
        return nextLevelUsers;
    }
    private async Task<int> GetNextUnqualifiedMatrixTypeAsync(int userId,
        IReadOnlyList<MatrixConfiguration> allMatrices)
    {
        var qualifications = await _matrixQualificationRepository.GetAllByUserIdAsync(userId);
        var matrixQualifications = qualifications as MatrixQualification[] ?? qualifications.ToArray();

        // Si no hay calificaciones, empezar con Matrix 1
        if (matrixQualifications.Length == 0)
            return 1;

        // Crear un diccionario para fácil acceso
        var qualificationDict = matrixQualifications.ToDictionary(q => q.MatrixType);

        // Encontrar el ciclo máximo alcanzado por cualquier matriz
        var maxCycle = matrixQualifications.Max(q => q.QualificationCount);

        // Buscar la primera matriz que no ha completado todos los ciclos hasta maxCycle
        foreach (var matrix in allMatrices.OrderBy(m => m.MatrixType).Select(m => m.MatrixType))
        {
            if (!qualificationDict.TryGetValue(matrix, out var qualification))
            {
                // Si no existe registro para esta matriz, es la siguiente
                return matrix;
            }

            // Si esta matriz está atrasada en ciclos
            if (qualification.QualificationCount < maxCycle)
            {
                // Si no está calificada en su ciclo actual, es la siguiente
                if (!qualification.IsQualified)
                {
                    return matrix;
                }

                // Si está calificada pero con menos ciclos que el máximo, 
                // necesita avanzar al siguiente ciclo
                return matrix;
            }

            // Si está en el ciclo máximo pero no calificada
            if (qualification.QualificationCount == maxCycle && !qualification.IsQualified)
            {
                return matrix;
            }
        }

        // Si todas las matrices están en el ciclo máximo y calificadas,
        // la siguiente es Matrix 1 del siguiente ciclo
        return 1;
    }

    private async Task<UserProcessingResult> ProcessSingleUserAsync(int userId, List<MatrixConfiguration> allMatrices)
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
    
    private async Task<(bool anyQualified, List<int> qualifiedMatrixTypes)>
        ProcessAllMatrixQualificationsAsync(int userId, List<MatrixConfiguration> allMatrices)
    {
        if (allMatrices == null || allMatrices.Count == 0)
            return (false, new List<int>());

        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        // 1) Datos financieros y calificaciones iniciales
        var (commissions, totalWithdrawn, availableBalance) = await GetFinancialsAsync(userId, brandId);
        var qualifications = await EnsureQualificationsAsync(userId, allMatrices, commissions, totalWithdrawn, availableBalance);

        // 2) Nombre de usuario
        var userName = (await _accountServiceAdapter.GetUserInfo(userId, brandId))?.UserName ?? "Usuario";

        // 3) Proceso secuencial por matriz 
        var anyQualified = false;
        var qualifiedMatrixTypes = new List<int>();
        var usersToVerify = new HashSet<int>();

        foreach (var m in allMatrices)
        {
            var q = qualifications[m.MatrixType];

            var minCycle = GetMinCycle(qualifications);
            if (q.QualificationCount > minCycle) continue;

            if (!await CheckQualificationAsync(userId, m.MatrixType)) continue;

            var result = await TryProcessMatrixAsync(userId, brandId, m, q, userName, availableBalance);
            if (!result.Success)
            {
                if (result.StopProcessing) break; // saldo insuficiente: detener
                continue; // error no crítico: seguir con la siguiente
            }

            // Actualizar estado a partir del resultado
            availableBalance = result.NewAvailableBalance;
            anyQualified = true;
            qualifiedMatrixTypes.Add(m.MatrixType);
            usersToVerify.UnionWith(result.CommissionRecipients);
        }

        // 4) Sincronizar saldos finales y verificar receptores
        await SyncBalancesAsync(qualifications.Values, availableBalance);

        if (usersToVerify.Count > 0)
            await VerifyRecipientQualificationsAsync(usersToVerify, 1, allMatrices);

        return (anyQualified, qualifiedMatrixTypes);
    }
    
    private async Task<(decimal commissions, decimal withdrawn, decimal available)>
        GetFinancialsAsync(int userId, long brandId)
    {
        var commissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId) ?? 0m;
        var totalWithdrawn = await _walletRequestRepository.GetTotalWithdrawnByAffiliateId(userId) ?? 0m;
        var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(userId, brandId);
        var pendingRequests = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(userId, brandId);

        return (commissions, totalWithdrawn, availableBalance - pendingRequests);
    }

    private async Task<Dictionary<int, MatrixQualification>> EnsureQualificationsAsync(
        int userId,
        IEnumerable<MatrixConfiguration> allMatrices,
        decimal commissions,
        decimal totalWithdrawn,
        decimal availableBalance)
    {
        var qualifications = (await _matrixQualificationRepository.GetAllByUserIdAsync(userId))
            .ToDictionary(q => q.MatrixType);

        var now = DateTime.Now;

        foreach (var matrixType in allMatrices.Select(m => m.MatrixType))
        {
            if (!qualifications.TryGetValue(matrixType, out var q))
            {
                q = new MatrixQualification
                {
                    UserId = userId,
                    MatrixType = matrixType,
                    TotalEarnings = commissions,
                    WithdrawnAmount = totalWithdrawn,
                    AvailableBalance = availableBalance,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _matrixQualificationRepository.CreateAsync(q);
                qualifications[matrixType] = q;
            }
            else
            {
                q.TotalEarnings = commissions;
                q.WithdrawnAmount = totalWithdrawn;
                q.AvailableBalance = availableBalance;
                q.UpdatedAt = now;
                await _matrixQualificationRepository.UpdateAsync(q);
            }
        }

        return qualifications;
    }

    private static int GetMinCycle(Dictionary<int, MatrixQualification> qualifications)
        => qualifications.Count == 0 ? 0 : qualifications.Values.Min(x => x.QualificationCount);

    private async Task<(bool Success, bool StopProcessing, decimal NewAvailableBalance, int[] CommissionRecipients)>
        TryProcessMatrixAsync(
            int userId,
            long brandId,
            MatrixConfiguration m,
            MatrixQualification q,
            string userName,
            decimal availableBalance)
    {
        try
        {
            // Débito + actualización en transacción
            var newAvailable = await ApplyQualificationAsync(q, m, userName, availableBalance);

            // Colocar usuario en matriz
            await _accountServiceAdapter.PlaceUserInMatrix(
                new MatrixRequest
                {
                    UserId = userId,
                    MatrixType = m.MatrixType
                },
                brandId);

            // Comisiones y cache
            var recipients = await ProcessMatrixCommissionsAsync(userId, m.MatrixType, q.QualificationCount);
            await _redisCache.InvalidateBalanceAsync(userId);
            await _redisCache.InvalidateBalanceAsync(recipients.ToArray());

            return (true, false, newAvailable, recipients.ToArray());
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Available Balance"))
        {
            _logger.LogInformation(ex,
                "User {UserId} could not qualify in matrix {MatrixType} due to insufficient balance",
                userId, m.MatrixType);

            return (false, true, availableBalance, []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing qualification for user {UserId} in matrix {MatrixType}", userId, m.MatrixType);
            return (false, false, availableBalance, []);
        }
    }

    private async Task SyncBalancesAsync(IEnumerable<MatrixQualification> qualifications, decimal finalAvailable)
    {
        var now = DateTime.Now;
        foreach (var qual in qualifications)
        {
            if (qual.AvailableBalance == finalAvailable) continue;
            qual.AvailableBalance = finalAvailable;
            qual.UpdatedAt = now;
            await _matrixQualificationRepository.UpdateAsync(qual);
        }
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
            var matrixConfig = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);

            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException($"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new InvalidDataException($"Error deserialize matrix configuration: {matrixConfigResponse.StatusCode}");

            // Verificar que el usuario tenga una posición válida en esta matriz
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(new MatrixRequest
            {
                UserId = userId,
                MatrixType = matrixType
            }, brandId);

            // Deserializamos la respuesta completa
            var matrixPositionResponse = JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!);
            // Extraemos la posición desde la propiedad Data
            var position = matrixPositionResponse?.Data;

            if (positionResponse.StatusCode != HttpStatusCode.OK || position == false)
            {
                throw new UnauthorizedAccessException($"User {userId} does not have a valid position in matrix {matrixType}");
            }

            // Calcular el 10% de comisión del monto de la tarifa
            var commissionAmount = matrixConfig.Data.FeeAmount * 0.1m;
            int paidCount = 0;
            
            // Obtener todas las posiciones superiores (upline)
            var uplinePositionsResponse = await _accountServiceAdapter.GetUplinePositionsAsync(
                new MatrixRequest
                {
                    UserId = userId,
                    MatrixType = matrixType,
                    Cycle = userQualificationCount
                }, brandId);

            var jObject = JObject.Parse(uplinePositionsResponse.Content!);
            var allUplinePositions = jObject["data"]?.ToObject<IEnumerable<MatrixPositionDto>>();

            if (uplinePositionsResponse.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException(
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
                _logger.LogInformation("Processing commissions for user {UserId} in matrix {MatrixType}. " +
                                       "Found {Count} eligible upline positions.", userId, matrixType, filteredPositions.Count);

                foreach (var uplineUserId in filteredPositions.Select(p => p.UserId))
                {
                    // Verificar si el upline está calificado en esta matriz y tiene al menos el mismo número de calificaciones
                    var uplineQualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(
                        uplineUserId, matrixType);

                    // Solo pagar si está calificado Y tiene al menos el mismo contador de calificaciones
                    if (uplineQualification is { IsQualified: true } &&
                        uplineQualification.QualificationCount >= userQualificationCount)
                    {
                        var now = DateTime.Now;

                        // Crear registro de ganancia para el upline calificado
                        var earning = new MatrixEarning
                        {
                            UserId = uplineUserId,
                            MatrixType = matrixType,
                            Amount = commissionAmount,
                            SourceUserId = userId,
                            EarningType = "Matrix_Qualification",
                            CreatedAt = now
                        };

                        // Pasar el contador de calificaciones al crear la ganancia
                        await _matrixEarningsRepository.CreateAsync(earning, userQualificationCount);
                        await _redisCache.InvalidateBalanceAsync(userId, (int)uplineUserId);

                        // Agregar este usuario a la lista de los que recibieron comisiones
                        usersReceivedCommissions.Add((int)uplineUserId);
                        paidCount++;
                    }
                }
                var missedCount = Math.Max(0, matrixConfig.Data.Levels - paidCount);
                if (missedCount > 0)
                {
                    var adminMissed = Math.Round(missedCount * commissionAmount, 2);

                    // Registro de earning (trazabilidad)
                    var adminEarning = new MatrixEarning {
                        UserId = 0,
                        MatrixType = matrixType,
                        Amount = adminMissed,
                        SourceUserId = userId,
                        EarningType = "Admin_MissedCommission",
                        CreatedAt = DateTime.Now
                    };
                    await _matrixEarningsRepository.CreateAsync(adminEarning, userQualificationCount);

                    // Asiento contable (wallet) como crédito
                    await _walletRepository.CreateAsync(new Wallet {
                        AffiliateId = 0,
                        UserId = 1,
                        Concept = $"Comisiones no pagadas x{missedCount} - {matrixConfig.Data.MatrixName}",
                        Detail = $"User {userId} • Ciclo {userQualificationCount}",
                        Debit = 0,
                        Credit = adminMissed,
                        AffiliateUserName = "adminrecycoin",
                        AdminUserName = "adminrecycoin",
                        Status = true,
                        ConceptType = "admin_missed_commission",
                        BrandId = brandId,
                        Date = DateTime.Now,
                    });
                }
            }

            await transaction.CommitAsync();
            return usersReceivedCommissions;
        }
        catch (Exception)
        {
            // En caso de error, revertir la transacción
            await transaction.RollbackAsync();
            throw; // Relanzar la excepción para manejo superior
        }
    }
    private async Task<bool> ProcessCoinPaymentsMatrixActivationAsync(int userId, int matrixType, int? recipientId = null)
    {
        try
        {
            var targetUserId = recipientId ?? userId;

            // 1. Obtener configuración de la matriz
            var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(
                _brandService.BrandId, matrixType);
            if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException(
                    $"Error al obtener configuración de matriz: {matrixConfigResponse.StatusCode}");

            var matrixConfig = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!)?.Data;
            if (matrixConfig == null)
                throw new InvalidDataException("La configuración de la matriz es inválida");

            // 2. Verificar si el usuario ya tiene una posición en esta matriz
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(
                new MatrixRequest
                {
                    UserId = targetUserId,
                    MatrixType = matrixType
                }, _brandService.BrandId);

            var existing = JsonConvert.DeserializeObject<MatrixPositionResponse>(positionResponse.Content!)?.Data;

            if (positionResponse.IsSuccessful && existing == true)
                // Si el usuario ya tiene una posición activa en la matriz, no se puede activar nuevamente
                return false;

            // 3. Obtener información de usuarios


            // 5. Crear o actualizar calificación
            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(targetUserId, matrixType);

            if (qualification?.IsQualified == true)
                return false;

            var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(targetUserId, _brandService.BrandId);
            var pendingAmount = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(targetUserId, _brandService.BrandId);
            availableBalance -= pendingAmount;

            if (qualification == null)
            {
                qualification = new MatrixQualification
                {
                    UserId = targetUserId,
                    MatrixType = matrixType,
                    TotalEarnings = matrixConfig.Threshold,
                    WithdrawnAmount = 0,
                    AvailableBalance = availableBalance,
                    IsQualified = true,
                    QualificationCount = 1,
                    LastQualificationTotalEarnings = matrixConfig.Threshold,
                    LastQualificationWithdrawnAmount = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastQualificationDate = DateTime.Now
                };
                await _matrixQualificationRepository.CreateAsync(qualification);
            }
            else
            {
                qualification.IsQualified = true;
                qualification.QualificationCount += 1;
                qualification.AvailableBalance = availableBalance;
                qualification.LastQualificationTotalEarnings = qualification.TotalEarnings;
                qualification.LastQualificationWithdrawnAmount = qualification.WithdrawnAmount;
                qualification.LastQualificationDate = DateTime.Now;
                qualification.UpdatedAt = DateTime.Now;
                await _matrixQualificationRepository.UpdateAsync(qualification);
            }

            // 6. Colocar usuario en la matriz
            await _accountServiceAdapter.PlaceUserInMatrix(new MatrixRequest
            {
                UserId = targetUserId,
                MatrixType = matrixType
            }, _brandService.BrandId);

            // 7. Procesar comisiones para uplines
            await ProcessMatrixCommissionsAsync(targetUserId, matrixType, qualification.QualificationCount);

            // 8. Invalidar cache
            await _redisCache.InvalidateBalanceAsync(targetUserId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CoinPayments matrix activation for user {UserId}, matrix {MatrixType}", userId, matrixType);
            return false;
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
                    "Fixed qualification record ID {QualificationId} for user {UserId}", record.QualificationId, record.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing qualification record ID {QualificationId} for user {UserId}", record.QualificationId, record.UserId);
            }
        }

        _logger.LogInformation("Process completed. Corrected {CorrectedCount} of {MatrixQualificationsCount} records.", correctedCount,
            matrixQualifications.Length);
    }
    public async Task<bool> CheckQualificationAsync(long userId, int matrixType)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        // 2.a Configuración de la matriz
        var cfgResp = await _configurationAdapter.GetMatrixConfiguration(brandId, matrixType);
        if (cfgResp.Content == null || cfgResp.StatusCode != HttpStatusCode.OK)
            throw new InvalidDataException($"Error retrieving matrix configuration: {cfgResp.StatusCode}");

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

        var progress = commissions;

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
                throw new InvalidOperationException(
                    $"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");

            if (matrixConfig?.Data is null)
                throw new InvalidDataException("Matrix configuration data is missing or invalid");

            // 2. Verificar si el usuario ya tiene una posición en esta matriz
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(new MatrixRequest
            {
                UserId = userId,
                MatrixType = matrixType
            }, _brandService.BrandId);
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
            _logger.LogWarning(ex, "Error in admin matrix placement: {Message}", ex.Message);
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
                throw new InvalidDataException(
                    $"Error al obtener configuración de matriz: {matrixConfigResponse.StatusCode}");

            var matrixConfig = JsonConvert
                .DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!)?
                .Data;
            if (matrixConfig == null)
                throw new InvalidOperationException("La configuración de la matriz es inválida");

            var availableBalance =
                await _walletRepository.GetAvailableBalanceByAffiliateId(request.UserId, _brandService.BrandId);
            var pendingAmount =
                await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(request.UserId,
                    _brandService.BrandId);
            availableBalance -= pendingAmount;
            if (availableBalance < matrixConfig.FeeAmount)
                return false;
            
            var adminBase = Math.Round(matrixConfig.FeeAmount * 0.30m, 2);
            var positionResponse = await _accountServiceAdapter.IsActiveInMatrix(
                new MatrixRequest
                {
                    UserId = targetUserId,
                    MatrixType = request.MatrixType
                }, _brandService.BrandId);

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
                throw new InvalidOperationException("No se pudo procesar el débito");
            
            await _walletRepository.CreateAsync(new Wallet {
                AffiliateId = 0,
                UserId = 1, 
                Concept = $"Fee admin 30% - {matrixConfig.MatrixName} (User {qualification.UserId})",
                Detail = $"Ciclo {qualification.QualificationCount + 1}",
                Debit = 0,
                Credit = adminBase,
                AffiliateUserName = Constants.RecycoinAdmin,
                AdminUserName = Constants.RecycoinAdmin,
                Status = true,
                ConceptType = "admin_fee",
                BrandId = _brandService.BrandId,
                Date = DateTime.Now,
            });
            
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

    public async Task<bool> CoinPaymentsMatrixActivationConfirmation(IpnRequest request, IHeaderDictionary headers)
    {
        var isValid = IsRequestValid(request, headers);

        if (!isValid)
            return false;

        var transactionResult = await _transactionRepository.GetTransactionByTxnId(request.txn_id);

        if (transactionResult is null)
            return false;

        if (transactionResult.Status == 100)
            return false;

        transactionResult.Status = request.status;
        transactionResult.AmountReceived = request.received_amount;
        var matrixType = transactionResult.Products.ExtractProductIdFromJson_JArray();

        if (request.status == -1)
        {
            transactionResult.Acredited = false;
            await _transactionRepository.UpdateTransactionAsync(transactionResult);
            return false;
        }

        if (!transactionResult.Acredited && request.status == 100)
        {
            try
            {
                var activationResult = await ProcessCoinPaymentsMatrixActivationAsync(transactionResult.AffiliateId, matrixType);

                if (activationResult)
                {
                    transactionResult.Acredited = true;
                    _logger.LogInformation(
                        "Matrix {MatrixType} activated successfully for user {AffiliateId} via CoinPayments transaction {TxnId}", matrixType,
                        transactionResult.AffiliateId, request.txn_id);
                }
                else
                {
                    _logger.LogWarning("Failed to activate matrix {MatrixType} for user {AffiliateId} via CoinPayments transaction {TxnId}", matrixType,
                        transactionResult.AffiliateId, request.txn_id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating matrix for CoinPayments transaction {TxnId}", request.txn_id);
                transactionResult.Acredited = false;
            }
        }

        await _transactionRepository.UpdateTransactionAsync(transactionResult);
        return true;
    }

    public async Task<bool> HasReachedWithdrawalLimitAsync(int userId)
    {
        var brandId = _brandService.BrandId == 0 ? 2 : _brandService.BrandId;

        // Cache key específica 
        var cacheKey = $"withdrawal_limit_{brandId}_{userId}";

        // Intentar obtener del cache
        var cachedResult = await _redisCache.Get<bool?>(cacheKey);
        if (cachedResult.HasValue)
        {
            _logger.LogDebug("Withdrawal limit retrieved from cache for user: {UserId}", userId);
            return cachedResult.Value;
        }

        try
        {
            var totalCommissions = await _walletRepository.GetQualificationBalanceAsync(userId, brandId) ?? 0m;

            var allMatrices = await _redisCache.Remember(
                $"matrix_cfg_{brandId}",
                TimeSpan.FromMinutes(10),
                async () =>
                {
                    var resp = await _configurationAdapter.GetAllMatrixConfigurations(brandId);
                    var data = JObject.Parse(resp.Content!)["data"]!.ToObject<List<MatrixConfiguration>>()!;
                    return data.OrderBy(m => m.MatrixType).ToList();
                });

            var nextMatrixType = await GetNextUnqualifiedMatrixTypeAsync(userId, allMatrices);

            var cfgResp = await _configurationAdapter.GetMatrixConfiguration(brandId, nextMatrixType);
            if (cfgResp.Content == null || cfgResp.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning("Error retrieving matrix configuration for type {NextMatrixType}: {StatusCode}", nextMatrixType, cfgResp.StatusCode);
                return false;
            }

            var cfg = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(cfgResp.Content!)?.Data;
            if (cfg == null)
            {
                _logger.LogWarning("Invalid matrix configuration for type {NextMatrixType}", nextMatrixType);
                return false;
            }

            var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, nextMatrixType);
            var cycle = qualification?.QualificationCount ?? 0;
            var goal = cycle == 0 ? cfg.Threshold : cfg.RangeMax * cycle;
            var withdrawalLimit = goal * 0.84m;
            var hasReachedLimit = totalCommissions >= withdrawalLimit;

            // Guardar en cache por 2 minutos
            await _redisCache.Set(cacheKey, hasReachedLimit, TimeSpan.FromMinutes(2));

            _logger.LogInformation("User {UserId} withdrawal limit check: Matrix {MatrixType}, Cycle {Cycle}," +
                                   " Total commissions: {TotalCommissions:C}, Goal: {Goal:C}, 84% Limit: {WithdrawalLimit:C}," +
                                   " Has reached limit: {HasReachedLimit}", userId, nextMatrixType, cycle, totalCommissions, goal,
                withdrawalLimit, hasReachedLimit);

            return hasReachedLimit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking withdrawal limit for user {UserId}", userId);
            return false;
        }
    }
}