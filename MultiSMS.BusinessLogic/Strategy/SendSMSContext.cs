namespace MultiSMS.BusinessLogic.Strategy
{
    public class SendSMSContext
    {
        private SendSmsStrategy? _smsStrategy;

        public void SetSmsStrategy(SendSmsStrategy smsStrategy)
        {
            _smsStrategy = smsStrategy;
        }

        public async Task<string> SendSMSAsync(string phone, string text, Dictionary<string, string> data)
        {
            if (_smsStrategy == null)
            {
                throw new Exception("Strategy has not been chosen");
            }

            return await _smsStrategy.SendSmsAsync(phone, text, data);
        }
    }
}
