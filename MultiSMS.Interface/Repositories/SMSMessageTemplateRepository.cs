using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class SMSMessageTemplateRepository : GenericRepository<SMSMessageTemplate>, ISMSMessageTemplateRepository
    {
        public SMSMessageTemplateRepository(MultiSMSDbContext context) : base(context)
        {
        }

        public async Task<SMSMessageTemplate> GetTemplateByNameAsync(string name)
        {
            return await GetAllEntries().FirstOrDefaultAsync(t => t.TemplateName == name) ?? throw new Exception($"Could not find template with given name: {name}");
        }
    }
}
