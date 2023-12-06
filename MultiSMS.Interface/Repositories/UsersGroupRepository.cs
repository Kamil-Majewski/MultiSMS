using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class UsersGroupRepository : IUsersGroupRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public UsersGroupRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UsersGroup> GetGroupByIdAsync(int groupId)
        {
            return await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId) ?? throw new Exception("Could not find group with given ID");
        }

        public async Task<UsersGroup> GetGroupByNameAsync(string groupName)
        {
            return await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName) ?? throw new Exception("Could not find group with given ID");
        }

        public async Task<IQueryable<UsersGroup>> GetAllGroupsAsync()
        {
            return await Task.FromResult(_dbContext.Groups);
        }

        public async Task<UsersGroup> CreateGroupAsync(UsersGroup group)
        {
            await _dbContext.AddAsync(group);
            await _dbContext.SaveChangesAsync();
            return group;
        }

        public async Task<UsersGroup> UpdateGroupAsync(UsersGroup group)
        {
            var groupFromDb = await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == group.GroupId);
            groupFromDb = group;
            await _dbContext.SaveChangesAsync();
            return groupFromDb;
        }

        public async Task DeleteGroupAsync(UsersGroup group)
        {
            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();
        }

    }
}
