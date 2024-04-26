using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class GroupService : GenericService<Group>, IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGenericRepository<Group> repository, IGroupRepository groupRepository) : base(repository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<Group> GetGroupByNameAsync(string groupName)
        {
            return await _groupRepository.GetGroupByNameAsync(groupName);
        }

        public List<Group> GetAllGroupsWithGroupMembersList()
        {
            return _groupRepository.GetAllGroupsWithGroupMembersQueryable().ToList();
        }

        public Dictionary<int, string> GetDictionaryWithGroupIdsAndNames()
        {
            return _groupRepository.GetDictionaryWithGroupIdsAndNames();
        }

        public async Task<(List<Group>, bool)> PaginateGroupDataAsync(int firstId, int lastId, int pageSize, bool? moveForward)
        {
            IQueryable<Group> query = _groupRepository.GetAllEntries().OrderBy(g => g.GroupId);

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
            return await _groupRepository.GetAllEntries().Where(g =>
            g.GroupName.ToLower().Contains(searchPhrase) ||
            (g.GroupDescription == null || g.GroupDescription.Equals(string.Empty) ? "Brak opisu" : g.GroupDescription!).ToLower().Contains(searchPhrase)).ToListAsync();
        }
    }
}
