using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("RoleId")]
    public class EmployeesRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = default!;

    }
}
