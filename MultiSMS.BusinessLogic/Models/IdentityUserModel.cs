using MultiSMS.BusinessLogic.Models.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;

namespace MultiSMS.BusinessLogic.Models
{
    public class IdentityUserModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string Surname { get; set; } = default!;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Phone]
        [StringLength(11, ErrorMessage = "Numer telefonu musi mieć {2} - {1} znaków.", MinimumLength = 11)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [Display(Name = "Hasło")]
        [StringLength(100, ErrorMessage = "{0} musi mieć {2} - {1} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [StrongPassword(ErrorMessage = "Hasło musi zawierać jedną cyfrę oraz wielką literę")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Rola jest wymagana")]
        public string Role { get; set; } = default!;
    }
}
