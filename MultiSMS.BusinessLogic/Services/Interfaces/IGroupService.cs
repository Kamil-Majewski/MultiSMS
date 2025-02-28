using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IGroupService : IGenericService<Group>
    {
        Task<List<Group>> GetAllGroupsWithGroupMembersListAsync();
        Task<Dictionary<int, string>> GetDictionaryWithGroupIdsAndNamesAsync();
        Task<Group> GetGroupByNameAsync(string groupName);
        Task<List<Group>> GetGroupsBySearchPhraseAsync(string searchPhrase);
        Task<(List<Group>, bool)> PaginateGroupDataAsync(int firstId, int lastId, int pageSize, bool? moveForward);
    }
}