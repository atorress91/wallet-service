using System.Text;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Core.ConPaymentsApi
{
    public class ConPaymentsApi : IConPaymentsApi
    {
        private readonly        string     _privateKey;
        private readonly        string     _publicKey;
        private readonly        Encoding   _encoding   = Encoding.UTF8;
        private readonly  HttpClient _httpClient = new HttpClient();
        public ConPaymentsApi(string privkey, string pubkey)
        {
            _privateKey = privkey;
            _publicKey  = pubkey;
            if (_privateKey.Length == 0 || _publicKey.Length == 0)
            {
                throw new ArgumentException("Private or Public Key is empty");
            }
        }

        public async Task<IRestResponse> CallApi(string cmd, SortedList<string, string>? parms)
        {
            if (parms == null)
            {
                parms = new SortedList<string, string>();
            }

            parms["version"] = "1";
            parms["key"]     = _publicKey;
            parms["cmd"]     = cmd;

            string postData = "";
            foreach (KeyValuePair<string, string> parm in parms)
            {
                if (postData.Length > 0)
                {
                    postData += "&";
                }

                postData += parm.Key + "=" + Uri.EscapeDataString(parm.Value);
            }

            byte[] keyBytes   = _encoding.GetBytes(_privateKey);
            byte[] postBytes  = _encoding.GetBytes(postData);
            var    hmacsha512 = new System.Security.Cryptography.HMACSHA512(keyBytes);
            string hmac       = BitConverter.ToString(hmacsha512.ComputeHash(postBytes)).Replace("-", string.Empty);

          
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("HMAC", hmac);

            var restResponse = new RestResponse();

            try
            {
                var content  = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _httpClient.PostAsync("https://www.coinpayments.net/api.php", content);

                restResponse.Content           = await response.Content.ReadAsStringAsync();
                restResponse.StatusCode        = response.StatusCode;
                restResponse.StatusDescription = response.ReasonPhrase;
            }
            catch (HttpRequestException e) 
            {
                restResponse.Content = "Exception while contacting CoinPayments.net: " + e.Message;
            }
            catch (Exception e)
            {
                restResponse.Content = "Unknown exception: " + e.Message;
            }

            return restResponse;
        }
    }
}