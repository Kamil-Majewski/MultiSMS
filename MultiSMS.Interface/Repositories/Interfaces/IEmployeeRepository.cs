using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
        Task<IQueryable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<IQueryable<Employee>> GetEmployeeBySurnameAsync(string surname);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
    }
}