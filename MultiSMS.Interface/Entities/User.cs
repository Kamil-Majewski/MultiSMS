using Microsoft.AspNetCore.Identity;

namespace MultiSMS.Interface.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string? Department {  get; set; } = default!;
        public int? DepartmentNumber { get; set; }
        public string? HQAddress { get; set; }
        public string? Role { get; set; }
    }
}
