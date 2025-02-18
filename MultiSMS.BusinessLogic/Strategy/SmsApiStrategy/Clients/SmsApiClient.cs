using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients
{
    public class SmsApiClient : ISmsClient
    {
        private readonly IApiSettingsService _settingsService;
        private readonly SmsApiSettings _secretToken;
        private readonly HttpClient _httpClient;
        private readonly string _system = "client_csharp";
        private readonly string _format = "json";

        public SmsApiClient(IApiSettingsService settingsService, IOptions<SmsApiSettings> secretToken, HttpClient httpClient)
        {
            _settingsService = settingsService;
            _secretToken = secretToken.Value;
            _httpClient = httpClient;
        }

        public async Task<string> SendSmsAsync(string phone, string text, string senderName)
        {
            // Fetch provider-specific settings from the database
            var apiSettings = await _settingsService.GetSettingsByNameAsync("ServerSms");

            var parameters = new Dictionary<string, string>
            {
                ["to"] = phone,
                ["message"] = text,
                ["format"] = _format,
                ["from"] = senderName,
                ["fast"] = apiSettings.FastChannel ? "1" : "0",
                ["test"] = apiSettings.TestMode ? "true" : "false",
                ["system"] = _system
            };

            string jsonData = JsonConvert.SerializeObject(parameters);

            using var request = new HttpRequestMessage(HttpMethod.Post, "sms.do/")
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretToken.ApiToken);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
