using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.Interface.Entities.ServerSms
{
    public class ServerSms
    {
        private const string _api_url = "https://api2.serwersms.pl/";
        private const string _system = "client_csharp";

        private string Username { get; set; } = default!;
        private string Password { get; set; } = default!;

        private string Format = "json";

        public ServerSms(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Authorization failed: username or password was empty!");
            }

            Username = username;
            Password = password;

        }

        public async Task<string> CallAsync(string url, Dictionary<string, string> data)
        {
            data["username"] = Username;
            data["password"] = Password;
            data["system"] = _system;

            string jsonData = JsonConvert.SerializeObject(data);

            using var httpClient = new HttpClient();
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync($"{_api_url}{url}.{Format}", requestContent);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody; //raw string response, process in controller or service

        }
    }
}
