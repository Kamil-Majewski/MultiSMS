using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities.ServerSms;

namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSmsTroughServerSms : SendSmsStrategy
    {
        private readonly ServerSmsSettings _smsSettings;
        private readonly IApiSettingsService _settingsService;

        public SendSmsTroughServerSms(IOptions<ServerSmsSettings> smsSettings, IApiSettingsService settingsService)
        {
            _smsSettings = smsSettings.Value;
            _settingsService = settingsService;
        }

        public override async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {
            var apiSettings = await _settingsService.GetSettingsByNameAsync("ServerSms");

            var serverSmsInstance = new ServerSms(_smsSettings.ApiToken);

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "phone", phone },
                    { "text", text },
                    { "details", "true" },
                    { "speed", apiSettings.FastChannel ? "1" : "0" },
                    { "test", apiSettings.TestMode ? "true" : "false" },
                    { "sender", $"{apiSettings.SenderName}" }
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("phone", phone);
                data.Add("text", text);
                data.Add("details", "true");
                data.Add("speed", apiSettings.FastChannel ? "1" : "0");
                data.Add("test", apiSettings.TestMode ? "true" : "false");
                data.Add("sender", $"{apiSettings.SenderName}" );
            }

            return await serverSmsInstance.CallAsync("messages/send_sms", data);
        }
    }
}
