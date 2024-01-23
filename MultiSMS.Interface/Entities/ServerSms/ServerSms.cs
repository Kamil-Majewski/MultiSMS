using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.Interface.Entities.ServerSms
{
    public class ServerSms
    {
        private const string _api_url = "https://api2.serwersms.pl/";
        private const string _system = "client_csharp";

        private string ApiToken { get; set; } = default!;

        private string Format = "json";

        public ServerSms(string apiToken)
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                throw new Exception("Authorization failed: token was empty!");
            }

            ApiToken = apiToken;

        }

        public async Task<string> CallAsync(string url, Dictionary<string, string> data)
        {
            data["system"] = _system;

            string jsonData = JsonConvert.SerializeObject(data);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiToken}");
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync($"{_api_url}{url}.{Format}", requestContent);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody; //raw string response, process in controller or service

        }
    }
}
