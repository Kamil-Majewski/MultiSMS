using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class EmployeeGroupService : IEmployeeGroupService
    {
        private readonly IEmployeeGroupRepository _repository;

        public EmployeeGroupService(IEmployeeGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task AddGroupMemberAsync(int groupId, int employeeId)
        {
            await _repository.AddGroupMemberAsync(groupId, employeeId);
        }

        public async Task<List<int>> GetAllEmployeesIdsForGroupListAsync(int groupId)
        {
            return await _repository.GetAllEmployeesIdsForGroupQueryable(groupId).ToListAsync();
        }

        public async Task<List<int>> GetAllGroupIdsForEmployeeListAsync(int employeeId)
        {
            return await _repository.GetAllGroupIdsForEmployeeQueryable(employeeId).ToListAsync();
        }

        public async Task RemoveGroupMember(int groupId, int employeeId)
        {
            await _repository.RemoveGroupMember(groupId, employeeId);
        }

        public async Task<List<string>> GetAllGroupNamesForEmployeeListAsync(int employeeId)
        {
            return await _repository.GetAllGroupNamesForEmployeeQueryable(employeeId).ToListAsync();
        }

        public async Task<List<string>> GetAllPhoneNumbersForGroupListAsync(int groupId)
        {
            return await _repository.GetAllPhoneNumbersForGroupQueryable(groupId).ToListAsync();
        }

        public async Task<List<string>> GetAllActiveEmployeesPhoneNumbersForGroupListAsync(int groupId)
        {
            return await _repository.GetAllActiveEmployeesPhoneNumbersForGroupQueryable(groupId).ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesForGroupListAsync(int groupId)
        {
            return await _repository.GetAllEmployeesForGroupQueryable(groupId).ToListAsync();
        }

        public Dictionary<int, IEnumerable<string>> GetDictionaryOfEmployeeIdAndGroupNames()
        {
            return _repository.GetDictionaryOfEmployeeIdAndGroupNames();
        }
    }
}
