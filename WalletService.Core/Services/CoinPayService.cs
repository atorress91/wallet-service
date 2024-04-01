using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class CoinPayService : BaseService, ICoinPayService
{
    private readonly ICoinPayAdapter _coinPayAdapter;
    private readonly ICoinPaymentTransactionRepository _coinPaymentTransactionRepository;
    private readonly string _sharedKey;

    public CoinPayService(IMapper mapper, ICoinPayAdapter coinPayAdapter,
        ICoinPaymentTransactionRepository coinPaymentTransactionRepository,
        IOptions<ApplicationConfiguration> appSettings) : base(mapper)
    {
        _coinPayAdapter = coinPayAdapter;
        _coinPaymentTransactionRepository = coinPaymentTransactionRepository;
        _sharedKey = appSettings.Value.CoinPay!.SecretKey!;
    }

    #region coingPay

    public async Task<CreateTransactionResponse?> CreateTransaction(CreateTransactionRequest request)
    {
        var today = DateTime.Now;
        var paymentRequest = new PaymentRequest
        {
            Amount = request.Amount,
            IdCurrency = Constants.UsdtIdCurrency,
            Details = JsonConvert.SerializeObject(request.Products)
        };

        var response = await _coinPayAdapter.CreateTransaction(paymentRequest);

        if (response.Content is null)
            return new CreateTransactionResponse();

        var result = response.Content!.ToJsonObject<CreateTransactionResponse>() ??
                     new CreateTransactionResponse();

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = result.Data!.IdTransaction.ToString(),
            AffiliateId = request.AffiliateId,
            Amount = result.Data!.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = result.Data.Details!,
            Acredited = false,
            Status = result.StatusCode,
            PaymentMethod = "CoinPay",
            CreatedAt = today,
            UpdatedAt = today
        };

        await _coinPaymentTransactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return result;
    }

    public async Task<CreateChannelResponse?> CreateChannel(CreateTransactionRequest request)
    {
        var today = DateTime.Now;
        PaymentTransaction? transactionResponse;
    
        var channelRequest = new CreateChannelRequest
        {
            IdCurrency = Constants.UsdtIdCurrency,
            IdExternalIdentification = request.AffiliateId,
            IdNetwork = Constants.UsdtIdNetwork,
            TagName = request.UserName,
        };

        var channelResponse = await _coinPayAdapter.CreateChannel(channelRequest);

        if (channelResponse.Content is null)
            return null;

        var channel = channelResponse.Content.ToJsonObject<CreateChannelResponse>();

        if (channel is null)
            return null;

        if (channel.StatusCode != 1)
            return null;
    
        var existingTransaction = await _coinPaymentTransactionRepository.GetCoinPaymentTransactionByIdTransaction(channel.Data!.Id.ToString());

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = channel.Data!.Id.ToString(),
            AffiliateId = request.AffiliateId,
            Amount = request.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = JsonConvert.SerializeObject(request.Products),
            Acredited = false,
            Status = Constants.EmptyValue,
            PaymentMethod = "CoinPay",
            CreatedAt = today,
            UpdatedAt = today,
            Reference = null
        };
        
        if (existingTransaction != null && existingTransaction.Acredited == false)
        {
            existingTransaction.Amount = request.Amount;
            existingTransaction.Products = JsonConvert.SerializeObject(request.Products);
            existingTransaction.UpdatedAt = today;
            
            transactionResponse = await _coinPaymentTransactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        }
        else
        {
            transactionResponse = await _coinPaymentTransactionRepository.CreateCoinPaymentTransaction(paymentTransaction);
        }
    
        if (transactionResponse is null)
            return null;

        return channel;
    }
    public async Task<GetNetworkResponse?> GetNetworksByIdCurrency(int idCurrency)
    {
        var result = await _coinPayAdapter.GetNetworksByIdCurrency(idCurrency);

        if (result.Content is null)
            return null;

        var networkResult = result.Content.ToJsonObject<GetNetworkResponse>();

        return networkResult;
    }

    public async Task<CreateAddressResponse?> CreateAddress(CreateAddresRequest request)
    {
        var result = await _coinPayAdapter.CreateAddress(Constants.CoinPayIdWallet, request);

        if (result.Content is null)
            return null;

        var address = result.Content.ToJsonObject<CreateAddressResponse>();

        if (address is null)
            return null;

        return address;
    }

    public async Task<bool> IsValidSignature(int idUser, int idTransaction, string dynamicKey, string incomingSignature)
    {
        var generatedSignature = GenerateSignature(idUser, idTransaction, dynamicKey);
        return await Task.FromResult(incomingSignature == generatedSignature);
    }

    private string GenerateSignature(int idUser, int idTransaction, string dynamicKey)
    {
        var tmpInfo = $"{idUser}{idTransaction}{dynamicKey}";
        using var hmacsha512 = new HMACSHA512(Encoding.ASCII.GetBytes(_sharedKey));
        var hashBytes = hmacsha512.ComputeHash(Encoding.ASCII.GetBytes(tmpInfo));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
    
    public async Task<GetTransactionByIdResponse?>GetTransactionById(int idTransaction)
    {
        var result = await _coinPayAdapter.GetTransactionById(idTransaction);

        if (result.Content is null)
            return null;

        var transaction = result.Content.ToJsonObject<GetTransactionByIdResponse>();

        return transaction;
    }

    #endregion
}