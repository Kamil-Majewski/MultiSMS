using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeGroupService : IGenericService<EmployeeGroup>
    {
        Task AddGroupMemberAsync(int groupId, int employeeId);
        Task<List<string>> GetAllActiveEmployeesPhoneNumbersForGroupListAsync(int groupId);
        Task<List<Employee>> GetAllEmployeesForGroupListAsync(int groupId);
        Task<List<int>> GetAllEmployeesIdsForGroupListAsync(int groupId);
        Task<List<int>> GetAllGroupIdsForEmployeeListAsync(int employeeId);
        Task<List<string>> GetAllGroupNamesForEmployeeListAsync(int employeeId);
        Task<List<string>> GetAllPhoneNumbersForGroupListAsync(int groupId);
        Task<Dictionary<int, IEnumerable<string>>> GetDictionaryOfEmployeeIdAndGroupNamesAsync();
        Task RemoveGroupMemberAsync(int groupId, int employeeId);
    }
}