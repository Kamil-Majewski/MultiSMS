using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Models.CustomException;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using System.Security.Claims;

namespace MultiSMS.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IAdministratorRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IUserStore<User> _userStore;

        public UserService(IAdministratorRepository adminRepository, IMapper mapper, UserManager<User> userManager, IUserStore<User> userStore)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
            _userManager = userManager;
            _emailStore = GetEmailStore();
            _userStore = userStore;
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
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

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'");
            }
        }

        public async Task<ManageUserDTO> CreateNewIdentityUser(IdentityUserModel model)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
            user.Name = model.Name;
            user.Surname = model.Surname;
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

                await _userManager.AddClaimAsync(user, new Claim("Password", "ChangePasswordOnLogin"));

                var currentUser = await _userManager.FindByNameAsync(user.UserName!) ?? throw new Exception($"Could not find a user by the username {user.UserName}");

                var roleResult = await _userManager.AddToRoleAsync(currentUser, model.Role);

                if (roleResult.Succeeded)
                {
                    var mappedUser = _mapper.Map<ManageUserDTO>(currentUser);
                    mappedUser.Role = model.Role;

                    return mappedUser;
                }
                else
                {
                    var errorMessages = roleResult.Errors.Select(e => e.Description).ToList();
                    throw new CustomValidationException("User creation failed", errorMessages);
                }
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                throw new CustomValidationException("User creation failed", errorMessages);
            }
        }

        public async Task<ManageUserDTO> EditIdenitityUser(int userId, IdentityUserModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception($"Could not find a user with provided Id {userId}");

            if (user.Name != model.Name)
            {
                user.Name = model.Name;
            }
            if (user.Surname != model.Surname)
            {
                user.Surname = model.Surname;
            }
            if (user.Email != model.Email)
            {
                user.UserName = model.Email;
                user.Email = model.Email;
            }
            if (user.PhoneNumber != model.PhoneNumber)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, userRole);
                await _userManager.AddToRoleAsync(user, model.Role);
                user.Role = model.Role;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return _mapper.Map<ManageUserDTO>(user);
            }
            else
            {
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                throw new CustomValidationException("User update failed", errorMessages);
            }
        }

        public async Task DeleteIdentityUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NullReferenceException($"Could not find a user with Id {userId}");
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await _userManager.UpdateAsync(user);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("There was an unexpecter error when attempting to delete user");
            }
        }
    }
}
