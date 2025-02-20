using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Models;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<ManageUserDTO> CreateNewUserAsync(IdentityUserModel model);
        Task DeleteUserAsync(int userId);
        Task<ManageUserDTO> EditUserAsync(int userId, IdentityUserModel model);
        Task<UserDTO> GetUserDtoByEmailAsync(string email);
        Task<UserDTO> GetAdministratorDtoByIdAsync(int id);
        Task<IEnumerable<ManageUserDTO>> GetAllManageUserDtosAsync();
        Task<ManageUserDTO> GetManageUserDtoByIdAsync(int userId);
        Task<string> GetUserRoleByIdAsync(int userId);
    }
}