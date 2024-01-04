using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeeGroupRepository : IEmployeeGroupRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeeGroupRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddGroupMemberAsync(int groupId, int employeeId)
        {
            await _dbContext.AddAsync(new EmployeeGroup { EmployeeId = employeeId, GroupId = groupId });
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<int> GetAllEmployeesIdsForGroupQueryable(int groupId)
        {
            return _dbContext.EmployeeGroups.Where(eg => eg.GroupId == groupId).Select(eg => eg.EmployeeId);
        }

        public async Task RemoveGroupMember(int groupId, int employeeId)
        {
            var groupMember = await _dbContext.EmployeeGroups.FirstOrDefaultAsync(eg => eg.GroupId == groupId && eg.EmployeeId == employeeId) ?? throw new Exception("Could not find the group with provided id that contains employee with given id");
            _dbContext.EmployeeGroups.Remove(groupMember);
            await _dbContext.SaveChangesAsync();
        }
    }
}
