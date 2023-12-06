using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeRoleRepository
    {
        Task<EmployeeRole> CreateEmployeeRoleAsync(EmployeeRole role);
        Task DeleteEmployeeRoleAsync(EmployeeRole roleToBeDeleted);
        Task<IQueryable<EmployeeRole>> GetAllEmployeeRoles();
        Task<EmployeeRole> GetEmployeeRoleByIdAsync(int roleId);
        Task<EmployeeRole> UpdateEmployeeRoleAsync(EmployeeRole updatedRole);
    }
}