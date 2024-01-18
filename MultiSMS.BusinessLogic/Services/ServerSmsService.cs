using Microsoft.Extensions.Options;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services
{
    public class ServerSmsService : IServerSmsService
    {
        private readonly ServerSmsSettings _smsSettings;

        public ServerSmsService(IOptions<ServerSmsSettings> smsSettings)
        {
            _smsSettings = smsSettings.Value;
        }

        private ServerSms CreateServerSMSInstance()
        {
            return new ServerSms(_smsSettings.Username, _smsSettings.Password);
        }

        public async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {

            var serverSmsInstance = CreateServerSMSInstance();

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "phone", phone },
                    { "text", text }
                    //{ "sender", "Toruń WOL" }
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("phone", phone);
                data.Add("text", text);
                //data.Add("sender", "Toruń WOL");
            }

            return await serverSmsInstance.CallAsync("messages/send_sms", data);
        }
    }
}
