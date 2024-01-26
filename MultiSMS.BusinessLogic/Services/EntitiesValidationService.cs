using Microsoft.IdentityModel.Tokens;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using System.Text.RegularExpressions;

namespace MultiSMS.BusinessLogic.Services
{
    public class EntitiesValidationService : IEntitiesValidationService
    {
        public bool CheckEmployeeValidity(Employee employee)
        {
            var phoneNumberPattern = "^(\\+[0-9]{2} )?\\d{3} \\d{3} \\d{3}$";
            Regex regex = new Regex(phoneNumberPattern);

            if (employee.Name.IsNullOrEmpty() || employee.Surname.IsNullOrEmpty() || employee.PhoneNumber.IsNullOrEmpty())
            {
                return false;
            }

            Match match = regex.Match(employee.PhoneNumber);

            if (!match.Success)
            {
                return false;
            }

            return true;

        }
    }
}
