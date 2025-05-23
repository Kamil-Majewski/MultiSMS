﻿using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
using MultiSMS.Interface.Entities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients
{
    public class ServerSmsClient : ISmsClient
    {
        private readonly IApiSettingsService _settingsService;
        private readonly HttpClient _httpClient;
        private readonly string _system = "client_csharp";
        private readonly string _format = "json";

        public ServerSmsClient(IApiSettingsService settingsService, HttpClient httpClient)
        {
            _settingsService = settingsService;
            _httpClient = httpClient;
        }

        public async Task<SendSmsResultModel> SendSmsAsync(string phone, string text, ApiSmsSender sender)
        {
            // Fetch provider-specific settings from the database
            var apiSettings = await _settingsService.GetSettingsByNameAsync("ServerSms");

            var parameters = new Dictionary<string, object>
            {
                ["phone"] = phone,
                ["text"] = text,
                ["details"] = "true",
                ["sender"] = sender.Name,
                ["speed"] = apiSettings.FastChannel ? "1" : "0",
                ["test"] = apiSettings.TestMode ? "true" : "false",
                ["system"] = _system
            };

            string jsonData = JsonConvert.SerializeObject(parameters);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"messages/send_sms.{_format}")
            {
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sender.ApiToken.Value);

            using var response = await _httpClient.SendAsync(request);

            return new SendSmsResultModel(await response.Content.ReadAsStringAsync(), parameters);
        }
    }
}
