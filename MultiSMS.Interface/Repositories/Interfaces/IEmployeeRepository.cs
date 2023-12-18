using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee> GetByNameAsync(string name);
    }
}