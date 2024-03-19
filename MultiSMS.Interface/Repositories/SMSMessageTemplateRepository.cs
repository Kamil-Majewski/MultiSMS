using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class SMSMessageTemplateRepository : GenericRepository<SMSMessageTemplate>, ISMSMessageTemplateRepository
    {
        public SMSMessageTemplateRepository(MultiSMSDbContext context) : base(context)
        {
        }
    }
}
