using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Core.ConPaymentsApi;

public interface IConPaymentsApi
{
    Task<IRestResponse> CallApi(string cmd, SortedList<string, string>? parms);
}