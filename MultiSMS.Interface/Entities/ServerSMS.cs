using Newtonsoft.Json;
using System.Text;
using System.Xml;

namespace MultiSMS.Interface.Entities
{
    public class ServerSms
    {
        private const string _api_url = "https://api2.serwersms.pl/";
        private const string _system = "client_csharp";

        private string Username { get; set; } = default!;
        private string Password { get; set; } = default!;

        public string Format = "json";

        public ServerSms(string username, string password)
        {
                if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Brak danych autoryzacyjnych");
            }

            Username = username;
            Password = password;

        }

        public async Task<object> CallAsync(string url, Dictionary<string, string> data)
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

            if (Format == "xml")
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseBody);
                return xmlDoc;
            }
            else
            {
                return JsonConvert.DeserializeObject(responseBody)!;
            }
        }
    }
}
