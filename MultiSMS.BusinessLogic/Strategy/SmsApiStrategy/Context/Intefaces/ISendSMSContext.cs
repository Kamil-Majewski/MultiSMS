using MultiSMS.BusinessLogic.Models;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces
{
    public interface ISendSMSContext
    {
        Task<SendSmsResultModel> SendSMSAsync(string phone, string text, string senderName);
    }
}