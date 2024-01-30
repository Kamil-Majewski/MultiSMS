using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<Group> GetGroupByNameAsync(string groupName);
        IQueryable<Group> GetAllGroupsWithGroupMembersQueryable();
        IQueryable<int> GetAllGroupIds();
    }
}