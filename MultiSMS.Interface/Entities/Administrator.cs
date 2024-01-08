using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    public class Administrator : IdentityUser<int>
    {
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
    }
}
