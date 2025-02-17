namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces
{
    public interface ISendSMSContext
    {
        Task<string> SendSMSAsync(string phone, string text);
    }
}