using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class SMSMessageTemplateService : GenericService<SMSMessageTemplate>, ISMSMessageTemplateService
    {

        public SMSMessageTemplateService(IGenericRepository<SMSMessageTemplate> repository) : base(repository)
        {
        }

        public async Task<SMSMessageTemplate> GetTemplateByNameAsync(string name)
        {
            ValidationHelper.ValidateString(name, nameof(name));

            return await GetAllEntriesQueryable().FirstOrDefaultAsync(t => t.TemplateName == name) ?? throw new Exception($"Could not find template with given name: {name}");
        }

        public async Task<(List<SMSMessageTemplate>, bool)> PaginateTemplateDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            ValidationHelper.ValidateId(firstId, nameof(firstId));
            ValidationHelper.ValidateId(lastId, nameof(lastId));
            ValidationHelper.ValidateId(pageSize, nameof(pageSize));

            IQueryable<SMSMessageTemplate> query = GetAllEntriesQueryable().OrderBy(t => t.TemplateId);

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
            ValidationHelper.ValidateString(searchPhrase, nameof(searchPhrase));

            return await GetAllEntriesQueryable().Where(t => 
            t.TemplateName.ToLower().Contains(searchPhrase) ||
            (t.TemplateDescription == null || t.TemplateDescription.Equals(string.Empty) ? "Brak opisu" : t.TemplateDescription!).ToLower().Contains(searchPhrase) ||
            t.TemplateContent.ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
