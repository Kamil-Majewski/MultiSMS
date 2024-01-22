using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MultiSMS.Interface.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

#nullable disable

namespace MultiSMS.MVC.Areas.Identity.Pages.Account
{
    [Authorize]
    public class ChangePasswordOnLogin : PageModel
    {
        private readonly UserManager<Administrator> _userManager;

        public ChangePasswordOnLogin(UserManager<Administrator> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło jednorazowe")]
            public string OldPassword { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "{0} musi mieć {2} - {1} znaków.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nowe hasło")]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.RemoveClaimAsync(user, new Claim("Password", "ChangePasswordOnLogin"));
                return LocalRedirect(Url.Content("~/"));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Zmiana hasła nie powiodła się.");
                return Page();
            }
        }
    }
}
