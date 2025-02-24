using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces;
using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory.Interface;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context
{
    public class SendSMSContext : ISendSMSContext
    {
        private readonly ISmsClientFactory _strategyFactory;

        public SendSMSContext(ISmsClientFactory strategyFactory)
        {
            _strategyFactory = strategyFactory;
        }

        public async Task<SendSmsResultModel> SendSMSAsync(string phone, string text, string senderName)
        {
            ValidationHelper.ValidateString(phone, nameof(phone));
            ValidationHelper.ValidateString(text, nameof(text));
            ValidationHelper.ValidateString(senderName, nameof(senderName));

            var client = await _strategyFactory.GetClientAsync();
            return await client.SendSmsAsync(phone, text, senderName);
        }
    }
}
