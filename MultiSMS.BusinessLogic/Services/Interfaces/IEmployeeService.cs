using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeService : IGenericService<Employee>
    {
        Task<Employee> GetEmployeeByNameAsync(string name);
    }
}