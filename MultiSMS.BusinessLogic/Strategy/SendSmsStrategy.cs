namespace MultiSMS.BusinessLogic.Strategy
{
    public abstract class SendSmsStrategy
    {
        public abstract Task<string> SendSmsAsync(string phone, string text, Dictionary<string, string> data);
    }
}
