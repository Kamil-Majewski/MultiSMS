using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeGroupService
    {
        Task AddGroupMemberAsync(int groupId, int employeeId);
        Task<List<string>> GetAllActiveEmployeesPhoneNumbersForGroupListAsync(int groupId);
        Task<List<Employee>> GetAllEmployeesForGroupListAsync(int groupId);
        Task<List<int>> GetAllEmployeesIdsForGroupListAsync(int groupId);
        Task<List<int>> GetAllGroupIdsForEmployeeListAsync(int employeeId);
        Task<List<string>> GetAllGroupNamesForEmployeeListAsync(int employeeId);
        Task<List<string>> GetAllPhoneNumbersForGroupListAsync(int groupId);
        Task RemoveGroupMember(int groupId, int employeeId);
        Dictionary<int, IEnumerable<string>> GetDictionaryOfEmployeeIdAndGroupNames();
    }
}