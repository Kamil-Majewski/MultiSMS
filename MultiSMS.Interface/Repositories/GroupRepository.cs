using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly MultiSMSDbContext _context;
        public GroupRepository(MultiSMSDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Group> GetGroupByNameAsync(string groupName)
        {
            return await GetAllEntries().FirstOrDefaultAsync(g => g.GroupName == groupName) ?? throw new Exception($"Could not find group with given group name: {groupName}");
        }

        public IQueryable<Group> GetAllGroupsWithGroupMembersQueryable()
        {
            return _context.Groups.Include(g => g.GroupMembers)!.ThenInclude(gm => gm.Employee);
        }

        public Dictionary<int, string> GetDictionaryWithGroupIdsAndNames()
        {
            return _context.Groups.ToDictionary(g => g.GroupId, g => g.GroupName);
        }
    }
}

