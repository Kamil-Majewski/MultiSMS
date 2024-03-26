using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories;
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

        public async Task<(List<Group>, bool)> PaginateGroupDataAsync(int lastId, int pageSize, bool moveForward)
        {
            IQueryable<Group> query;

            if (moveForward)
            {
                query = _groupRepository.GetAllEntries().OrderBy(t => t.GroupId).Where(t => t.GroupId > lastId);
            }
            else
            {
                query = _groupRepository.GetAllEntries().OrderBy(t => t.GroupId).Where(t => t.GroupId <= lastId);
            }
            var hasMorePages = _groupRepository.GetAllEntries().OrderBy(t => t.GroupId).Where(t => t.GroupId > lastId).Count() > pageSize;
            var paginatedList = await query.Take(pageSize).ToListAsync();

            return (paginatedList, hasMorePages);
        }
    }
}
