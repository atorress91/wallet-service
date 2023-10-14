using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WalletService.Core.Services;

public class CoinPayService:BaseService,ICoinPayService
{
    private readonly ICoinPayAdapter _coinPayAdapter;
    private readonly ICoinPaymentTransactionRepository _coinPaymentTransactionRepository;
    private readonly string _sharedKey;
    private readonly ApplicationConfiguration _appSettings;
    public CoinPayService(IMapper mapper,ICoinPayAdapter coinPayAdapter,ICoinPaymentTransactionRepository coinPaymentTransactionRepository,IOptions<ApplicationConfiguration> appSettings) : base(mapper)
    {
        _appSettings                      = appSettings.Value;
        _coinPayAdapter                   = coinPayAdapter;
        _coinPaymentTransactionRepository = coinPaymentTransactionRepository;
        _sharedKey                        = _appSettings.CoinPay!.SecretKey!;
    }

    #region coingPay
    
    public async Task<CreateTransactionResponse?> CreateTransaction(CreateTransactionRequest request)
    {
        var today = DateTime.Now;
        var paymentRequest = new PaymentRequest
        {
            Amount = request.Amount,
            IdCurrency = Constants.UsdtIdCurrency,
            Details = JsonSerializer.Serialize(request.Products)
        };
        
        var response                   = await _coinPayAdapter.CreateTransaction(paymentRequest);

        if (response.Content is null)
            return new CreateTransactionResponse();
                
        var result= JsonSerializer.Deserialize<CreateTransactionResponse>(response.Content!) ??
            new CreateTransactionResponse();

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction  = result.Data?.IdTransaction.ToString(),
            AffiliateId    = request.AffiliateId,
            Amount         = result.Data!.Amount,
            AmountReceived = Constants.None,
            Products       = result.Data.Details!,
            Acredited      = false,
            Status         = result.StatusCode,
            PaymentMethod  = "CoinPay",
            CreatedAt      = today,
            UpdatedAt      = today
        };

        await _coinPaymentTransactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return result;
    }
    
    public async Task<CreateChannelResponse?> CreateChannel(CreateTransactionRequest request)
    {
        var today  = DateTime.Now;
        var channelRequest = new CreateChannelRequest
        {
            IdCurrency               = Constants.UsdtIdCurrency,
            IdExternalIdentification = request.AffiliateId,
            IdNetwork                = Constants.UsdtIdNetwork,
            TagName                  = JsonSerializer.Serialize(request.Products),
        };
        
        var channelResponse = await _coinPayAdapter.CreateChannel(channelRequest);

        if (channelResponse.Content is null) 
            return null;

        var channel = JsonSerializer.Deserialize<CreateChannelResponse>(channelResponse.Content);

        if (channel is null)
            return null;
        
        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction  = channel.Data?.Id.ToString(),
            AffiliateId    = request.AffiliateId,
            Amount         = request.Amount,
            AmountReceived = Constants.None,
            Products       = channelRequest.TagName,
            Acredited      = false,
            Status         = channel.StatusCode,
            PaymentMethod  = "CoinPay",
            CreatedAt      = today,
            UpdatedAt      = today
        };

        await _coinPaymentTransactionRepository.CreateCoinPaymentTransaction(paymentTransaction);
        
        return channel;
    }
    public async Task<GetNetworkResponse?> GetNetworksByIdCurrency(int idCurrency)
    {
        var result = await _coinPayAdapter.GetNetworksByIdCurrency(idCurrency);

        if (result.Content is null)
            return null;

        var networkResult = JsonSerializer.Deserialize<GetNetworkResponse>(result.Content);

        return networkResult;
    }

    public async Task<CreateAddressResponse?> CreateAddress(CreateAddresRequest request)
    {
        var result = await _coinPayAdapter.CreateAddress(Constants.CoinPayIdWallet,request);

        if (result.Content is null)
            return null;
        
        var address = JsonSerializer.Deserialize<CreateAddressResponse>(result.Content);

        if (address is null)
            return null;
        
        return address;
    }
    
    public async Task<bool> IsValidSignature(int idUser, int idTransaction, string dynamicKey, string incomingSignature)
    {
        var generatedSignature = GenerateSignature(idUser, idTransaction, dynamicKey);
        return await Task.FromResult(incomingSignature == generatedSignature);
    }

    public async Task ProcessTransactionAsync(TransactionNotificationRequest transaction)
    {
       
    }

    private string GenerateSignature(int idUser, int idTransaction, string dynamicKey)
    {
        var tmpInfo = $"{idUser}{idTransaction}{dynamicKey}";
        using var hmacsha512 = new HMACSHA512(Encoding.ASCII.GetBytes(_sharedKey));
        var hashBytes = hmacsha512.ComputeHash(Encoding.ASCII.GetBytes(tmpInfo));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    
    #endregion
}