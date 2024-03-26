using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<(List<SMSMessageTemplate>, bool)> PaginateTemplateDataAsync(int lastId, int pageSize, bool moveForward)
        {
            IQueryable<SMSMessageTemplate> query;
            bool hasMorePages;

            if (moveForward)
            {
                query = _repository.GetAllEntries().OrderBy(t => t.TemplateId).Where(t => t.TemplateId > lastId);
                hasMorePages = query.Count() > pageSize;
            }
            else
            {
                query = _repository.GetAllEntries().OrderBy(t => t.TemplateId).Where(t => t.TemplateId <= lastId);
                hasMorePages = true;

            }
            var paginatedList = await query.Take(pageSize).ToListAsync();

            return (paginatedList, hasMorePages);
        }
    }
}
