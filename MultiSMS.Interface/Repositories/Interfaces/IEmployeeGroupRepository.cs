using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeGroupRepository
    {
        Task AddGroupMemberAsync(int groupId, int employeeId);
        IQueryable<int> GetAllEmployeesIdsForGroupQueryable(int groupId);
        Task RemoveGroupMember(int groupId, int employeeId);
    }
}