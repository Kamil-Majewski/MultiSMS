using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;

namespace MultiSMS.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IAdministratorRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(IAdministratorRepository adminRepository, IMapper mapper, UserManager<User> userManager)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<UserDTO> GetAdministratorDtoByEmailAsync(string email)
        {
            var admin = await _adminRepository.GetAdministratorByEmailAsync(email);
            return _mapper.Map<UserDTO>(admin);
        }

        public async Task<UserDTO> GetAdministratorDtoByIdAsync(int id)
        {
            var admin = await _adminRepository.GetAdinistratorByIdAsync(id);
            return _mapper.Map<UserDTO>(admin);
        }

        public async Task<IEnumerable<ManageUserDTO>> GetAllIdentityUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                user.Role = userRoles[0];
            }
            return _mapper.Map<IEnumerable<ManageUserDTO>>(users);
        }

        public async Task<ManageUserDTO> GetIdentityUserById(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception($"Could not find a user by provided Id {userId}");
            var roles = await _userManager.GetRolesAsync(user);
            user.Role = roles[0];

            return _mapper.Map<ManageUserDTO>(user);
        }

        public async Task<string> GetUserRoleById(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception($"Could not find a user with provided Id {userId}");
            var roles = await _userManager.GetRolesAsync(user);

            return roles[0];
        }
    }
}
