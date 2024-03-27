using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IGroupService : IGenericService<Group>
    {
        List<Group> GetAllGroupsWithGroupMembersList();
        Dictionary<int, string> GetDictionaryWithGroupIdsAndNames();
        Task<Group> GetGroupByNameAsync(string groupName);
        Task<(List<Group>, bool)> PaginateGroupDataAsync(int firstId, int lastId, int pageSize, bool moveForward);
    }
}