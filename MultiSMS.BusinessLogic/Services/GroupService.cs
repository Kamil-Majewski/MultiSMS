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
    }
}
