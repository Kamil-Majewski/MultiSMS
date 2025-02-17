using MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Clients.Interface;
namespace MultiSMS.BusinessLogic.Strategy.SmsApiStrategy.Factory.Interface
{
    public interface ISmsClientFactory
    {
        Task<ISmsClient> GetClientAsync();
    }
}
