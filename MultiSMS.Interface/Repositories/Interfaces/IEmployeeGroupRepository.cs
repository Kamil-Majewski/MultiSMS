namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IEmployeeGroupRepository
    {
        Task AddGroupMemberAsync(int groupId, int employeeId);
        IQueryable<int> GetAllEmployeesIdsForGroupQueryable(int groupId);
        Task RemoveGroupMember(int groupId, int employeeId);
        IQueryable<string> GetAllGroupNamesForEmployeeQueryable(int employeeId);
        IQueryable<string> GetAllPhoneNumbersForGroupQueryable(int groupId);
        IQueryable<string> GetAllActiveEmployeesPhoneNumbersForGroupQueryable(int groupId);
    }
}