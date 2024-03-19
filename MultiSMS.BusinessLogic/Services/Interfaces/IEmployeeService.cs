using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByNameAsync(string name);
    }
}