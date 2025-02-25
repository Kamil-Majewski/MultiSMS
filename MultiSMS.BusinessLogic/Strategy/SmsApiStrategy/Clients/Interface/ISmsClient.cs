using MultiSMS.BusinessLogic.Models;
using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface
{
    public interface ISmsClient
    {
        Task<SendSmsResultModel> SendSmsAsync(string phone, string text, ApiSmsSender sender);
    }
}
