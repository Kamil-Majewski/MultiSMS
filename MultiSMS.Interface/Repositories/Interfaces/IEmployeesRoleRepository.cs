using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeesRoleRepository
    {
        Task<EmployeesRole> CreateEmployeeRoleAsync(EmployeesRole role);
        Task DeleteEmployeeRoleAsync(EmployeesRole roleToBeDeleted);
        Task<IQueryable<EmployeesRole>> GetAllEmployeeRoles();
        Task<EmployeesRole> GetEmployeeRoleByIdAsync(int roleId);
        Task<EmployeesRole> UpdateEmployeeRoleAsync(EmployeesRole updatedRole);
    }
}