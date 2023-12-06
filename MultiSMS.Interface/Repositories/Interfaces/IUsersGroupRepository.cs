using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IUsersGroupRepository
    {
        Task<UsersGroup> CreateGroupAsync(UsersGroup group);
        Task DeleteGroupAsync(UsersGroup group);
        Task<UsersGroup> GetGroupByIdAsync(int groupId);
        Task<UsersGroup> GetGroupByNameAsync(string groupName);
        Task<UsersGroup> UpdateGroupAsync(UsersGroup group);
    }
}