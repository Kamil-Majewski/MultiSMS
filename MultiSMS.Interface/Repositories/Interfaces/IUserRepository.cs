using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IQueryable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<IQueryable<User>> GetUsersBySurnameAsync(string surname);
    }
}