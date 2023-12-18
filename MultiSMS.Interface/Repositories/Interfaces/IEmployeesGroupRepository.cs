using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeesGroupRepository : IGenericRepository<EmployeesGroup>
    {
        Task<EmployeesGroup> GetGroupByNameAsync(string groupName);
    }
}