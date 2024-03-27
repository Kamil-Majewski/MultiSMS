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

        public async Task<(List<Group>, bool)> PaginateGroupDataAsync(int firstId, int lastId, int pageSize, bool moveForward)
        {
            List<Group> paginatedList;
            bool hasMorePages;

            if (moveForward)
            {
                var query = _groupRepository.GetAllEntries().OrderBy(g => g.GroupId).Where(g => g.GroupId > lastId);
                paginatedList = await query.Take(pageSize).ToListAsync();
                hasMorePages = query.Count() > pageSize;
            }
            else
            {
                paginatedList = await _groupRepository.GetAllEntries().OrderBy(g => g.GroupId).Reverse().Where(g => g.GroupId < firstId).Take(pageSize).Reverse().ToListAsync();
                hasMorePages = _groupRepository.GetAllEntries().OrderBy(g => g.GroupId).Where(g => g.GroupId > paginatedList.Last().GroupId).Count() > 0;
            }
            return (paginatedList, hasMorePages);
        }
    }
}
