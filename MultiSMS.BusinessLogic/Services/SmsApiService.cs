using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities.SmsApi;

namespace MultiSMS.BusinessLogic.Services
{
    public class SmsApiService : ISmsApiService
    {
        private readonly SmsApiSettings _smsSettings;

        public SmsApiService(IOptions<SmsApiSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        public async Task<string> SendSmsAsync(string to, string message, Dictionary<string, string> data)
        {

            var serverSmsInstance = new SmsApi(_smsSettings.ApiToken);

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "to", to },
                    { "message", message },
                    { "format", "json" },
                    { "from", "Toruń WOL" },
                    { "fast", "1" },
                    { "test",  "true"}
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("to", to);
                data.Add("message", message);
                data.Add("format", "json");
                data.Add("from", "Toruń WOL");
                data.Add("fast", "1");
                data.Add("test", "true");
            }

            return await serverSmsInstance.CallAsync("sms.do", data);
        }
    }
}
