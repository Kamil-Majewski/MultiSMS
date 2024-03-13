using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSmsTroughSmsApi : SendSmsStrategy
    {
        private readonly ISmsApiService _smsApiService;

        public SendSmsTroughSmsApi(ISmsApiService smsApiService)
        {
                _smsApiService = smsApiService;
        }

        public override async Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data)
        {
            return await _smsApiService.SendSmsAsync(phone, text, data);
        }
    }
}
