using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.Interface.Entities.SmsApi
{
    public class SmsApi
    {
        private const string _api_url = "https://api.smsapi.pl/";
        private const string _system = "client_csharp";

        private string ApiToken { get; set; } = default!;


        public SmsApi(string _apiToken)
        {
            if (string.IsNullOrEmpty(_apiToken))
            {
                throw new Exception("Authorization failed: token was empty!");
            }

            ApiToken = _apiToken;
        }

        public async Task<string> CallAsync(string url, Dictionary<string, string> data)
        {
            data["authorization"] = $"Bearer {ApiToken}";
            data["system"] = _system;

            string jsonData = JsonConvert.SerializeObject(data);

            using var httpClient = new HttpClient();
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync($"{_api_url}{url}", requestContent);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }


    }
}
