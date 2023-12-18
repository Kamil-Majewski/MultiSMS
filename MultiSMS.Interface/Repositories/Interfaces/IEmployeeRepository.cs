using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByNameAsync(string name);
    }
}