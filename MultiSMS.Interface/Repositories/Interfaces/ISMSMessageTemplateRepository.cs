using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface ISMSMessageTemplateRepository : IGenericRepository<SMSMessageTemplate>
    {
        Task<SMSMessageTemplate> GetTemplateByNameAsync(string name);
    }
}