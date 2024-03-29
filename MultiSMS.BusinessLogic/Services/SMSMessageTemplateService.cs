using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiSMS.BusinessLogic.Extensions;
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

        public async Task<(List<SMSMessageTemplate>, bool)> PaginateTemplateDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            IQueryable<SMSMessageTemplate> query = _repository.GetAllEntries().OrderBy(t => t.TemplateId);

            List<SMSMessageTemplate> paginatedList;
            bool hasMorePages;

            if (moveForward == null)
            {
                query = query.Where(t => t.TemplateId >= firstId);
            }
            else if (moveForward == true)
            {
                query = query.Where(t => t.TemplateId > lastId);
            }
            else
            {
                paginatedList = await query.Reverse().Where(t => t.TemplateId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = await query.AnyAsync(t => t.TemplateId > paginatedList.Last().TemplateId);

                return (paginatedList, hasMorePages);
            }

            paginatedList = await query.Take(pageSize).ToListAsync();
            hasMorePages = await query.Skip(pageSize).AnyAsync();

            return (paginatedList, hasMorePages);
        }

        public async Task<List<SMSMessageTemplate>> GetTemplatesBySearchPhraseAsync(string searchPhrase)
        {
            return await _repository.GetAllEntries().Where(t => 
            t.TemplateName.ToLower().Contains(searchPhrase) ||
            (t.TemplateDescription == null || t.TemplateDescription.Equals(string.Empty) ? "Brak opisu" : t.TemplateDescription!).ToLower().Contains(searchPhrase) ||
            t.TemplateContent.ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
