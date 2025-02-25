using MultiSMS.BusinessLogic.Models;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Context.Intefaces
{
    public interface ISendSMSContext
    {
        Task<SendSmsResultModel> SendSMSAsync(string phone, string text, ApiSmsSender sender);
    }
}