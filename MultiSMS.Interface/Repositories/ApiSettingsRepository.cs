using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class ApiSettingsRepository : GenericRepository<ApiSettings>, IApiSettingsRepository
    {
        public ApiSettingsRepository(MultiSMSDbContext context) : base(context)
        {
        }
    }
}
