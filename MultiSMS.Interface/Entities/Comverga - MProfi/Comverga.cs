using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.Interface.Entities.Comverga___MProfi
{
    public class Comverga
    {
        private const string _api_url = "https://api.mprofi.pl/1.0/";

        private string ApiToken { get; set; } = default!;

        public Comverga(string apiToken)
        {
            if (string.IsNullOrEmpty(apiToken))
            {
                throw new Exception("Authorization failed: token was empty!");
            }

            ApiToken = apiToken;
        }

        public async Task<string> CallAsync(string url, Dictionary<string, string> data)
        {
            data["apikey"] = ApiToken;

            string jsonData = JsonConvert.SerializeObject(data);

            using var httpClient = new HttpClient();
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync($"{_api_url}{url}.json", requestContent);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody; //raw string response, process in controller or service
        }
    }
}
