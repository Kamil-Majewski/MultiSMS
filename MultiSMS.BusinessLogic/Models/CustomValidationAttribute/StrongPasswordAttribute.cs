using System.ComponentModel.DataAnnotations;

namespace MultiSMS.BusinessLogic.Models.CustomValidationAttribute
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string password))
            {
                return false;
            }

            return ContainsUppercaseLetter(password) && ContainsDigit(password);
        }

        private bool ContainsUppercaseLetter(string password)
        {
            return password.Any(char.IsUpper);
        }

        private bool ContainsDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}
