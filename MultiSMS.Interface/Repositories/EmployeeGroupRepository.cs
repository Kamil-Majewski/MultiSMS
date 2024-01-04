using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeeGroupRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeeGroupRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddGroupMemberAsync(int groupId, int employeeId)
        {
            await _dbContext.AddAsync(new EmployeeGroup {EmployeeId = employeeId, GroupId = groupId});
            await _dbContext.SaveChangesAsync();
        }
    }
}
