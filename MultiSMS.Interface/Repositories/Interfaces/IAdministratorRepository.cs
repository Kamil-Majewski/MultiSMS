using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IAdministratorRepository
    {
        Task<User> GetAdministratorByEmailAsync(string email);
        Task<User> GetAdinistratorByIdAsync(int id);
    }
}