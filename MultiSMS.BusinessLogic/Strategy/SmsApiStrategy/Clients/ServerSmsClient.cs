using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients
{
    public class ServerSmsClient : ISmsClient
    {
        private readonly HttpClient _httpClient;
        private readonly IApiSettingsService _settingsService;
        private readonly string _apiUrl = "https://api2.serwersms.pl/";
        private readonly string _system = "client_csharp";
        private readonly string _format = "json";

        public ServerSmsClient(HttpClient httpClient, IApiSettingsService settingsService)
        {
            _httpClient = httpClient;
            _settingsService = settingsService;
        }

        public async Task<string> SendSmsAsync(string phone, string text)
        {
            // Fetch provider-specific settings from the database
            var apiSettings = await _settingsService.GetSettingsByNameAsync("ServerSms");

            var parameters = new Dictionary<string, string>();
            parameters["phone"] = phone;
            parameters["text"] = text;
            parameters["details"] = "true";
            parameters["speed"] = apiSettings.FastChannel ? "1" : "0";
            parameters["test"] = apiSettings.TestMode ? "true" : "false";
            parameters["sender"] = apiSettings.SenderName;

            // Additional system parameter
            parameters["system"] = _system;

            string jsonData = JsonConvert.SerializeObject(parameters);
            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync($"{_apiUrl}/messages/send_sms.{_format}", requestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
