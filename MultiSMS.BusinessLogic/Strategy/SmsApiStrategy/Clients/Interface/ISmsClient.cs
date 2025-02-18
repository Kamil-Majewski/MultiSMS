using MultiSMS.BusinessLogic.Models;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface
{
    public interface ISmsClient
    {
        Task<SendSmsResultModel> SendSmsAsync(string phone, string text, string senderName);
    }
}
