using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IEmployeeService : IGenericService<Employee>
    {
        Task<Employee> GetEmployeeByNameAsync(string name);
        Task<(List<Employee>, bool)> PaginateEmployeeDataAsync(int lastId, int pageSize, bool moveForward);
    }
}