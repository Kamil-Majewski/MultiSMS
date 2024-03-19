using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ISMSMessageTemplateService
    {
        Task<SMSMessageTemplate> GetTemplateByNameAsync(string name);
    }
}