using MultiSMS.BusinessLogic.DTO;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<AdministratorDTO> GetAdministratorByEmailAsync(string email);
    }
}