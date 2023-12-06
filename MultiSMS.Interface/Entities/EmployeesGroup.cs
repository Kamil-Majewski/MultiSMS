using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("GroupId")]
    public class EmployeesGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public string? GroupDescription { get; set; }
        public ICollection<Administrator> GroupMembers { get; set; } = new List<Administrator>();

    }
}
