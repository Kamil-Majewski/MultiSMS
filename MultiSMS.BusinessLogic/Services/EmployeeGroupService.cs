using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class EmployeeGroupService : GenericService<EmployeeGroup>, IEmployeeGroupService
    {

        public EmployeeGroupService(IGenericRepository<EmployeeGroup> repository) : base(repository)
        {
        }

        public async Task AddGroupMemberAsync(int groupId, int employeeId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));
            ValidationHelper.ValidateId(employeeId, nameof(employeeId));

            await AddEntityToDatabaseAsync(new EmployeeGroup { EmployeeId = employeeId, GroupId = groupId });
        }

        public async Task<List<int>> GetAllEmployeesIdsForGroupListAsync(int groupId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));

            return await GetAllEntriesQueryable().Where(eg => eg.GroupId == groupId)
                                        .Select(eg => eg.EmployeeId)
                                        .ToListAsync();
        }

        public async Task<List<int>> GetAllGroupIdsForEmployeeListAsync(int employeeId)
        {
            ValidationHelper.ValidateId(employeeId, nameof(employeeId));

            return await GetAllEntriesQueryable().Where(eg => eg.EmployeeId == employeeId)
                                        .Select(eg => eg.GroupId)
                                        .ToListAsync();
        }

        public async Task RemoveGroupMemberAsync(int groupId, int employeeId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));
            ValidationHelper.ValidateId(employeeId, nameof(employeeId));

            var employeeGroup = await GetAllEntriesQueryable().FirstOrDefaultAsync(eg => eg.GroupId == groupId && eg.EmployeeId == employeeId)
                                                     ?? throw new Exception("Could not find the group with provided id that contains employee with given id");

            await DeleteEntityByIdAsync(employeeGroup.EmployeeId);
        }

        public async Task<List<string>> GetAllGroupNamesForEmployeeListAsync(int employeeId)
        {
            ValidationHelper.ValidateId(employeeId, nameof(employeeId));

            return await GetAllEntriesQueryable().Where(eg => eg.EmployeeId == employeeId)
                                        .Include(eg => eg.Group)
                                        .Select(g => g.Group.GroupName)
                                        .ToListAsync();
        }

        public async Task<List<string>> GetAllPhoneNumbersForGroupListAsync(int groupId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));

            return await GetAllEntriesQueryable().Where(eg => eg.GroupId == groupId)
                                        .Include(eg => eg.Employee)
                                        .Select(e => e.Employee.PhoneNumber)
                                        .ToListAsync();
        }

        public async Task<List<string>> GetAllActiveEmployeesPhoneNumbersForGroupListAsync(int groupId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));

            return await GetAllEntriesQueryable().Where(eg => eg.GroupId == groupId)
                                        .Include(eg => eg.Employee)
                                        .Where(e => e.Employee.IsActive == true)
                                        .Select(e => e.Employee.PhoneNumber)
                                        .ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesForGroupListAsync(int groupId)
        {
            ValidationHelper.ValidateId(groupId, nameof(groupId));

            return await GetAllEntriesQueryable().Where(eg => eg.GroupId == groupId)
                                        .Include(eg => eg.Employee)
                                        .Select(e => e.Employee)
                                        .ToListAsync();
        }


        public async Task<Dictionary<int, IEnumerable<string>>> GetDictionaryOfEmployeeIdAndGroupNamesAsync()
        {
            return await GetAllEntriesQueryable().Include(eg => eg.Group)
                                        .GroupBy(eg => eg.EmployeeId)
                                        .ToDictionaryAsync(
                                            group => group.Key,
                                            group => group.Select(eg => eg.Group.GroupName)
                                        );
        }
    }
}
