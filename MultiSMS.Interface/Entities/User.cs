using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiSMS.Interface.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;

        [NotMapped]
        public string? Role { get; set; }
    }
}
