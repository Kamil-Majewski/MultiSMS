using MultiSMS.BusinessLogic.DTO;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetAdministratorDtoByEmailAsync(string email);
        Task<UserDTO> GetAdministratorDtoByIdAsync(int id);
    }
}