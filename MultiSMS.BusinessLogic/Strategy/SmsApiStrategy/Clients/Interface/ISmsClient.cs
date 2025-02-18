namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface
{
    public interface ISmsClient
    {
        Task<string> SendSmsAsync(string phone, string text, string senderName);
    }
}
