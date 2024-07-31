using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Models;

namespace MultiSMS.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<ManageUserDTO> CreateNewIdentityUser(IdentityUserModel model);
        Task DeleteIdentityUser(int userId);
        Task<ManageUserDTO> EditIdenitityUser(int userId, IdentityUserModel model);
        Task<UserDTO> GetAdministratorDtoByEmailAsync(string email);
        Task<UserDTO> GetAdministratorDtoByIdAsync(int id);
        Task<IEnumerable<ManageUserDTO>> GetAllIdentityUsers();
        Task<ManageUserDTO> GetIdentityUserById(int userId);
        Task<string> GetUserRoleById(int userId);
    }
}