using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class GroupService : GenericService<Group>, IGroupService
    {
        public GroupService(IGenericRepository<Group> groupRepository) : base(groupRepository)
        {
        }

        public async Task<Group> GetGroupByNameAsync(string groupName)
        {
            ValidationHelper.ValidateString(groupName, nameof(groupName));

            return await GetAllEntriesQueryable().FirstOrDefaultAsync(g => g.GroupName == groupName) ?? throw new Exception($"Could not find group with given group name: {groupName}");
        }

        public async Task<List<Group>> GetAllGroupsWithGroupMembersListAsync()
        {
            return await GetAllEntriesQueryable().Include(g => g.GroupMembers)!.ThenInclude(gm => gm.Employee).ToListAsync();
        }

        public async Task<Dictionary<int, string>> GetDictionaryWithGroupIdsAndNamesAsync()
        {
            return await GetAllEntriesQueryable().ToDictionaryAsync(g => g.GroupId, g => g.GroupName);
        }

        public async Task<(List<Group>, bool)> PaginateGroupDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            ValidationHelper.ValidateId(firstId, nameof(firstId));
            ValidationHelper.ValidateId(lastId, nameof(lastId));
            ValidationHelper.ValidateId(pageSize, nameof(pageSize));

            IQueryable<Group> query = GetAllEntriesQueryable().OrderBy(g => g.GroupId);

            List<Group> paginatedList;
            bool hasMorePages;

            if (moveForward == null)
            {
                query = query.Where(g => g.GroupId >= firstId);

            }
            else if (moveForward == true)
            {
                query = query.Where(g => g.GroupId > lastId);
            }
            else
            {
                paginatedList = await query.Reverse().Where(g => g.GroupId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = await query.AnyAsync(g => g.GroupId > paginatedList.Last().GroupId);

                return (paginatedList, hasMorePages);
            }

            paginatedList = await query.Take(pageSize).ToListAsync();
            hasMorePages = await query.Skip(pageSize).AnyAsync();

            return (paginatedList, hasMorePages);
        }

        public async Task<List<Group>> GetGroupsBySearchPhraseAsync(string searchPhrase)
        {
            ValidationHelper.ValidateString(searchPhrase, nameof(searchPhrase));

            return await GetAllEntriesQueryable().Where(g =>
            g.GroupName.ToLower().Contains(searchPhrase) ||
            (g.GroupDescription == null || g.GroupDescription.Equals(string.Empty) ? "Brak opisu" : g.GroupDescription!).ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
