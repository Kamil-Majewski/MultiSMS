using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeesGroupRepository : GenericRepository<EmployeesGroup>, IEmployeesGroupRepository
    {
        public EmployeesGroupRepository(MultiSMSDbContext context) : base(context)
        {
        }

        public async Task<EmployeesGroup> GetGroupByNameAsync(string groupName)
        {
            return await GetAllEntries().FirstOrDefaultAsync(g => g.GroupName == groupName) ?? throw new Exception($"Could not find group with given group name: {groupName}");
        }
    }
}

