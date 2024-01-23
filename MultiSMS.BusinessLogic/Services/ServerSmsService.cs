using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities.ServerSms;

namespace MultiSMS.BusinessLogic.Services
{
    public class ServerSmsService : IServerSmsService
    {
        private readonly ServerSmsSettings _smsSettings;

        public ServerSmsService(IOptions<ServerSmsSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        public async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {

            var serverSmsInstance = new ServerSms(_smsSettings.ApiToken);

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "phone", phone },
                    { "text", text },
                    { "details", "true"},
                    { "speed", "1"},
                    { "test", "true" },
                    { "sender", "TORUN WOL" }
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("phone", phone);
                data.Add("text", text);
                data.Add("details", "true");
                data.Add("speed", "1");
                data.Add("test", "true");
                data.Add("sender", "TORUN WOL");
            }

            return await serverSmsInstance.CallAsync("messages/send_sms", data);
        }
    }
}
