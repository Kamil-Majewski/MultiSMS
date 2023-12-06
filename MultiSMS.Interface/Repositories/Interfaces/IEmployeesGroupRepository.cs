using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeesGroupRepository
    {
        Task<EmployeesGroup> CreateGroupAsync(EmployeesGroup group);
        Task DeleteGroupAsync(EmployeesGroup group);
        Task<IQueryable<EmployeesGroup>> GetAllGroupsAsync();
        Task<EmployeesGroup> GetGroupByIdAsync(int groupId);
        Task<EmployeesGroup> GetGroupByNameAsync(string groupName);
        Task<EmployeesGroup> UpdateGroupAsync(EmployeesGroup group);
    }
}