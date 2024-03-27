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

        public async Task<(List<SMSMessageTemplate>, bool)> PaginateTemplateDataAsync(int firstId, int lastId, int pageSize, bool moveForward)
        {
            List<SMSMessageTemplate> paginatedList;
            bool hasMorePages;

            if (moveForward)
            {
                var query = _repository.GetAllEntries().OrderBy(t => t.TemplateId).Where(t => t.TemplateId > lastId);
                paginatedList = await query.Take(pageSize).ToListAsync();
                hasMorePages = query.Count() > pageSize;
            }
            else
            {
                paginatedList = await _repository.GetAllEntries().OrderBy(t => t.TemplateId).Reverse().Where(t => t.TemplateId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = _repository.GetAllEntries().OrderBy(t => t.TemplateId).Where(t => t.TemplateId > paginatedList.Last().TemplateId).Count() > 0;
            }

            return (paginatedList, hasMorePages);
        }
    }
}
