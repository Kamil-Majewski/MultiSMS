// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MultiSMS.Interface.Entities;

namespace MultiSMS.MVC.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Numer telefonu")]
            [Required(ErrorMessage = "Pole numer telefonu nie może być puste!")]
            public string PhoneNumber { get; set; }
            [MaxLength(20)]
            [Required(ErrorMessage="Pole imię nie może być puste!")]
            public string Name { get; set; }
            [MaxLength(20)]
            [Required(ErrorMessage = "Pole nazwisko nie może być puste!")]
            public string Surname { get; set; }
        }

        private void Load(User user)
        {
            Username = user.UserName;

            Input = new InputModel
            {
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var adminIdentity = await _userManager.GetUserAsync(User);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Błąd: Wystąpił nieoczekiwany błąd podczas zmiany numeru telefonu.";
                    return RedirectToPage();
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Profil został pomyślnie zaktualizowany";
                return RedirectToPage();
            }
            else if (Input.Name != adminIdentity.Name || Input.Surname != adminIdentity.Surname)
            {
                user.Name = Input.Name;
                user.Surname = Input.Surname;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Błąd: Wystąpił nieoczekiwany błąd podczas aktualizowania danych osobowych";
                }
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Profil został pomyślnie zaktualizowany";
                return RedirectToPage();
            }
            else
            {
                StatusMessage = "Nie zmieniono żadnych danych";
                return RedirectToPage();
            }

           
            
        }
    }
}
