namespace MultiSMS.BusinessLogic.Strategy.Intefaces
{
    public interface ISendSMSContext
    {
        Task<string> SendSMSAsync(string phone, string text, Dictionary<string, string> data);
        void SetSmsStrategy(SendSmsStrategy smsStrategy);
    }
}