using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiSMS.BusinessLogic.DTO;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Models;
using MultiSMS.BusinessLogic.Models.CustomException;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using System.Security.Claims;

namespace MultiSMS.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        public UserService(IMapper mapper, IUserStore<User> userStore, UserManager<User> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }

        public async Task<UserDTO> GetUserDtoByEmailAsync(string email)
        {
            ValidationHelper.ValidateString(email, nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserDtoByIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            var user = await _userManager.FindByIdAsync(userId.ToString());
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<ManageUserDTO>> GetAllManageUserDtosAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                user.Role = userRoles[0];
            }
            return _mapper.Map<IEnumerable<ManageUserDTO>>(users);
        }

        public async Task<ManageUserDTO> GetManageUserDtoByIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new Exception($"Could not find a user by provided Id {userId}");
            var roles = await _userManager.GetRolesAsync(user);
            user.Role = roles[0];

            return _mapper.Map<ManageUserDTO>(user);
        }

        public async Task<string> GetUserRoleByIdAsync(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

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

        public async Task<ManageUserDTO> CreateNewUserAsync(IdentityUserModel model)
        {
            ValidationHelper.ValidateObject(model, nameof(model));

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

        public async Task<ManageUserDTO> EditUserAsync(int userId, IdentityUserModel model)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));
            ValidationHelper.ValidateObject(model, nameof(model));

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

        public async Task DeleteUserAsync(int userId)
        {
            ValidationHelper.ValidateId(userId, nameof(userId));

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
