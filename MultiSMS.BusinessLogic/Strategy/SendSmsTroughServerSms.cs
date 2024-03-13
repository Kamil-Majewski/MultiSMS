using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSmsTroughServerSms : SendSmsStrategy
    {
        private readonly IServerSmsService _serverSmsService;

        public SendSmsTroughServerSms(IServerSmsService serverSmsService)
        {
            _serverSmsService = serverSmsService;
        }

        public override async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {
            return await _serverSmsService.SendSmsAsync(phone, text, data);
        }
    }
}
