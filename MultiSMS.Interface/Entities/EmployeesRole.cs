using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("RoleId")]
    public class EmployeesRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = default!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;

    }
}
