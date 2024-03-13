using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities.SmsApi;

namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSmsTroughSmsApi : SendSmsStrategy
    {
        private readonly SmsApiSettings _smsSettings;

        public SendSmsTroughSmsApi(IOptions<SmsApiSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        public override async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {
            var smsApiInstance = new SmsApi(_smsSettings.ApiToken);

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "to", phone },
                    { "message", text },
                    { "format", "json" },
                    { "from", "Toruń WOL" },
                    { "fast", "1" },
                    { "test",  "true"}
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("to", phone);
                data.Add("message", text);
                data.Add("format", "json");
                data.Add("from", "Toruń WOL");
                data.Add("fast", "1");
                data.Add("test", "true");
            }

            return await smsApiInstance.CallAsync("sms.do", data);
        }
    }
}
