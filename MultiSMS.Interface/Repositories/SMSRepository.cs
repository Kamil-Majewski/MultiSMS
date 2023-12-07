using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Repositories
{
    public class SMSRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public SMSRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }


    }
}
