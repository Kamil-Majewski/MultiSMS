using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeesGroupRepository
    {
        Task<EmployeesGroup> GetGroupByNameAsync(string groupName);
    }
}