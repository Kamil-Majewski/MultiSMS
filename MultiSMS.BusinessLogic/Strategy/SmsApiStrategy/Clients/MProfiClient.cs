using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using MultiSMS.Interface.Entities;
using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients
{
    public class MProfiClient : ISmsClient
    {
        private readonly HttpClient _httpClient;

        public MProfiClient(HttpClient httpClient, IOptions<MProfiSettings> secretTokens)
        {
            _httpClient = httpClient;
        }

        public async Task<SendSmsResultModel> SendSmsAsync(string phone, string text, ApiSmsSender sender)
        {
            var phonesParsed = phone.Contains(",") ? phone.Split(",") : [phone];

            var parameters = new Dictionary<string, object>()
            {
                ["apikey"] = sender.ApiToken.Value,
                ["recipients"] = phonesParsed,
                ["default_message"] = text
            };

            string jsonData = JsonConvert.SerializeObject(parameters);

            using var request = new HttpRequestMessage(HttpMethod.Post, "sendbulk/")
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request);

            return new SendSmsResultModel(await response.Content.ReadAsStringAsync(), parameters);
        }
    }
}
