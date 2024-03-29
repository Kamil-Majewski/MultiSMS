using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface ISMSMessageTemplateService : IGenericService<SMSMessageTemplate>
    {
        Task<SMSMessageTemplate> GetTemplateByNameAsync(string name);
        Task<(List<SMSMessageTemplate>, bool)> PaginateTemplateDataAsync(int firstId, int lastId, int pageSize, bool? moveForward);
        Task<List<SMSMessageTemplate>> GetTemplatesBySearchPhraseAsync(string searchPhrase);
    }
}