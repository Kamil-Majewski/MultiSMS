using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeesGroupRepository : IEmployeesGroupRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeesGroupRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployeesGroup> GetGroupByIdAsync(int groupId)
        {
            return await _dbContext.EmployeeGroups.FirstOrDefaultAsync(g => g.GroupId == groupId) ?? throw new Exception("Could not find group with given ID");
        }

        public async Task<EmployeesGroup> GetGroupByNameAsync(string groupName)
        {
            return await _dbContext.EmployeeGroups.FirstOrDefaultAsync(g => g.GroupName == groupName) ?? throw new Exception("Could not find group with given ID");
        }

        public async Task<IQueryable<EmployeesGroup>> GetAllGroupsAsync()
        {
            return await Task.FromResult(_dbContext.EmployeeGroups);
        }

        public async Task<EmployeesGroup> CreateGroupAsync(EmployeesGroup group)
        {
            await _dbContext.AddAsync(group);
            await _dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<EmployeesGroup> UpdateGroupAsync(EmployeesGroup group)
        {
            var groupFromDb = await _dbContext.EmployeeGroups.FirstOrDefaultAsync(g => g.GroupId == group.GroupId);
            groupFromDb = group;
            await _dbContext.SaveChangesAsync();
            return groupFromDb;
        }

        public async Task DeleteGroupAsync(EmployeesGroup group)
        {
            _dbContext.EmployeeGroups.Remove(group);
            await _dbContext.SaveChangesAsync();
        }

    }
}
