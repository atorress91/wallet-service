using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.PagaditoRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class PagaditoService : BaseService, IPagaditoService
{
    private readonly IPagaditoAdapter _pagaditoAdapter;
    private readonly ApplicationConfiguration _appSettings;
    private readonly ICoinPaymentTransactionRepository _transactionRepository;

    public PagaditoService(IOptions<ApplicationConfiguration> appSettings, IMapper mapper,
        IPagaditoAdapter pagaditoAdapter, ICoinPaymentTransactionRepository transactionRepository) : base(mapper)
    {
        _pagaditoAdapter = pagaditoAdapter;
        _appSettings = appSettings.Value;
        _transactionRepository = transactionRepository;
    }

    public async Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request)
    {
        var connectResponse = await _pagaditoAdapter.ConnectAsync();

        if (connectResponse == null || string.IsNullOrEmpty(connectResponse.Value))
            return "This connect is not valid";

        var pagaditoTransaction = Mapper.Map<CreatePagaditoTransaction>(request);
        pagaditoTransaction.Token = connectResponse.Value;
        pagaditoTransaction.Ern = Guid.NewGuid().ToString();

        var executeTransaction = await _pagaditoAdapter.ExecuteTransaction(pagaditoTransaction);

        if (executeTransaction == null || string.IsNullOrEmpty(executeTransaction.Value))
            return "This transaction is not valid";

        var productDetails = request.Details.Select(detail => new ProductRequest
        {
            ProductId = int.Parse(detail.UrlProduct!),
            Quantity = int.Parse(detail.Quantity!),
        }).ToList();

        var productsJson = JsonSerializer.Serialize(productDetails);

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = pagaditoTransaction.Ern,
            AffiliateId = request.AffiliateId,
            Amount = pagaditoTransaction.Amount,
            AmountReceived = Constants.None,
            Products = productsJson,
            PaymentMethod = Constants.Pagadito,
            Status = Constants.None,
            Acredited = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return executeTransaction.Value;
    }

    public async Task<bool> VerifySignature(IHeaderDictionary headers, string requestBody)
    {
        var notificationId = headers["PAGADITO-NOTIFICATION-ID"];
        var notificationTimestamp = headers["PAGADITO-NOTIFICATION-TIMESTAMP"];
        var certUrl = headers["PAGADITO-CERT-URL"];
        var notificationSignature = Convert.FromBase64String(headers["PAGADITO-SIGNATURE"]!);
        var wsk = _appSettings.Pagadito!.Wsk;

        var eventId = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(requestBody)?.id;

        var dataSigned = string.Join("|", notificationId, notificationTimestamp, eventId, CommonExtensions.Crc32(requestBody),
            wsk);

        byte[] certContent;
        using (var httpClient = new HttpClient())
        {
            certContent = await httpClient.GetByteArrayAsync(certUrl);
        }

        using var cert = new X509Certificate2(certContent);
        using var pubkey = cert.GetRSAPublicKey();

        var signatureResult = pubkey!.VerifyData(Encoding.UTF8.GetBytes(dataSigned), notificationSignature,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return signatureResult;
    }

    public async Task<bool> ProcessPurchase(WebHookRequest? request)
    {
        if (request is null)
            return false;

        var transaction = await _transactionRepository.GetCoinPaymentTransactionByIdTransaction(request.Resource!.Ern!);

        if (transaction == null)
            return false;

        if (request.Resource.Status == Constants.RegisteredStatus)
        {
            transaction.Status = Constants.RegisteredStatusCode;
            transaction.Acredited = false;
        }
        else if (request.Resource.Status == Constants.ExpiredStatus)
        {
            transaction.Status = Constants.ExpiredStatusCode;
            transaction.Acredited = false;
        }
        else if (request.Resource.Status == Constants.CompletedStatus)
        {
            transaction.Status = Constants.CompletedStatusCode;
            transaction.AmountReceived = decimal.Parse(request.Resource.Amount!.ToString()!);
            transaction.Acredited = true;
        }

        await _transactionRepository.UpdateCoinPaymentTransactionAsync(transaction);

        return true;
    }
}