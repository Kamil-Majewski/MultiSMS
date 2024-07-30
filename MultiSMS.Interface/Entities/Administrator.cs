using Microsoft.AspNetCore.Identity;

namespace MultiSMS.Interface.Entities
{
    public class Administrator : IdentityUser<int>
    {
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
    }
}
