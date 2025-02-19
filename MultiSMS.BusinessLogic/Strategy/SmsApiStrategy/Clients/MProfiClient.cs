using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using Newtonsoft.Json;
using System.Text;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients
{
    public class MProfiClient : ISmsClient
    {
        private readonly HttpClient _httpClient;
        private readonly MProfiSettings _secretTokens;

        public MProfiClient(HttpClient httpClient, IOptions<MProfiSettings> secretTokens)
        {
            _httpClient = httpClient;
            _secretTokens = secretTokens.Value;
        }

        public async Task<SendSmsResultModel> SendSmsAsync(string phone, string text, string senderName)
        {
            if (!_secretTokens.SenderNameTokenDictionary.TryGetValue(senderName, out var token))
            {
                throw new Exception("Unknown sender name for MProfi API");
            };

            var parameters = new Dictionary<string, string>()
            {
                ["apikey"] = token,
                ["recipients"] = phone,
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
