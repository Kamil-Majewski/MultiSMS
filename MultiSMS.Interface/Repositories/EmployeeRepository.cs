using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.Interface.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MultiSMSDbContext _dbContext;

        public EmployeeRepository(MultiSMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _dbContext.Employees.FirstOrDefaultAsync(u => u.EmployeeId == employeeId) ?? throw new Exception("Could not find employee with given ID");
        }

        public async Task<IQueryable<Employee>> GetEmployeeBySurnameAsync(string surname)
        {
            return await Task.FromResult(_dbContext.Employees.Where(u => u.Surname == surname));
        }

        public async Task<IQueryable<Employee>> GetAllEmployeesAsync()
        {
            return await Task.FromResult(_dbContext.Employees);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            var employeeFromDb = await GetEmployeeByIdAsync(employee.EmployeeId);
            employeeFromDb = employee;
            await _dbContext.SaveChangesAsync();
            return employeeFromDb;
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            await Task.FromResult(_dbContext.Employees.Remove(employee));
            await _dbContext.SaveChangesAsync();
        }

    }
}
