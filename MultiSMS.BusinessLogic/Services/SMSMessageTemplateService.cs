using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class SMSMessageTemplateService : GenericService<SMSMessageTemplate>, ISMSMessageTemplateService
    {
        private readonly IGenericRepository<SMSMessageTemplate> _repository;

        public SMSMessageTemplateService(IGenericRepository<SMSMessageTemplate> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<SMSMessageTemplate> GetTemplateByNameAsync(string name)
        {
            return await _repository.GetAllEntries().FirstOrDefaultAsync(t => t.TemplateName == name) ?? throw new Exception($"Could not find template with given name: {name}");
        }

        public async Task<List<SMSMessageTemplate>> PaginateTemplateDataAsync(int lastId, int pageSize)
        {
            return await _repository.GetAllEntries().OrderBy(t => t.TemplateId).Where(t => t.TemplateId > lastId).Take(pageSize).ToListAsync();
        }
    }
}
