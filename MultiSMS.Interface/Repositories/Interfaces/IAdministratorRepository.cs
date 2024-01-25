using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface.Repositories.Interfaces
{
    public interface IAdministratorRepository
    {
        Task<Administrator> GetAdministratorByEmailAsync(string email);
        Task<Administrator> GetAdinistratorByIdAsync(int id);
    }
}