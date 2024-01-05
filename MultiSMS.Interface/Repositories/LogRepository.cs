using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class LogRepository : GenericRepository<Log>, ILogRepository
    {
        public LogRepository(MultiSMSDbContext context) : base(context)
        {
        }
    }
}
