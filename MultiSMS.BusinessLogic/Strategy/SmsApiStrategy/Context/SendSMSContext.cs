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

        public async Task<string> SendSMSAsync(string phone, string text)
        {
            var client = await _strategyFactory.GetClientAsync();
            return await client.SendSmsAsync(phone, text);
        }
    }
}
