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

        public async Task<Object> SendSmsAsync(string phone, string text, string sender, Dictionary<string, string> data)
        {

            var serverSmsInstance = CreateServerSMSInstance();

            if (data == null)
            {
                var dataDictionary = new Dictionary<string, string>
                {
                    { "phone", phone },
                    { "text", text },
                    { "sender", sender }
                };

                data = dataDictionary;
            }
            else
            {
                data.Add("phone", phone);
                data.Add("text", text);
                data.Add("sender", sender);
            }

            return await serverSmsInstance.CallAsync("messages/send_sms", data);
        }
    }
}
