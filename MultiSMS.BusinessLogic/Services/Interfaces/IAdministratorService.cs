using MultiSMS.BusinessLogic.DTO;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<AdministratorDTO> GetAdministratorDtoByEmailAsync(string email);
        Task<AdministratorDTO> GetAdministratorDtoByIdAsync(int id);
    }
}