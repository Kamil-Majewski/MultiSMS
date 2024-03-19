using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities.SmsApi;

namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSmsTroughSmsApi : SendSmsStrategy
    {
        private readonly SmsApiSettings _smsSettings;
        private readonly IApiSettingsService _settingsService;

        public SendSmsTroughSmsApi(IOptions<SmsApiSettings> smsSettings, IApiSettingsService settingsService)
        {
            _smsSettings = smsSettings.Value;
            _settingsService = settingsService;
        }

        public override async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {
            var smsApiInstance = new SmsApi(_smsSettings.ApiToken);
            var apiSettings = await _settingsService.GetSettingsByNameAsync("SmsApi");

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "to", phone },
                    { "message", text },
                    { "format", "json" },
                    { "from", $"{apiSettings.SenderName}" },
                    { "fast", apiSettings.FastChannel ? "1" : "0" },
                    { "test",  apiSettings.TestMode ? "true" : "false"}
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("to", phone);
                data.Add("message", text);
                data.Add("format", "json");
                data.Add("from", $"{apiSettings.SenderName}" );
                data.Add("fast", apiSettings.FastChannel ? "1" : "0" );
                data.Add("test", apiSettings.TestMode ? "true" : "false");
            }

            return await smsApiInstance.CallAsync("sms.do", data);
        }
    }
}
