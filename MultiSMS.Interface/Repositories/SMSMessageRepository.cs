using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class SMSMessageRepository : GenericRepository<SMSMessage>, ISMSMessageRepository
    {
        public SMSMessageRepository(MultiSMSDbContext context) : base(context)
        {
        }
    }
}
