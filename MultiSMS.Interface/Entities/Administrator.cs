using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("AdministratorId")]
    public class Administrator : IdentityUser<int>
    {
        public int AdministratorId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
    }
}
