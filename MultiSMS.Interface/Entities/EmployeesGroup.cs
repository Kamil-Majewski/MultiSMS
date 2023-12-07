using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("GroupId")]
    public class EmployeesGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public string? GroupDescription { get; set; }
        public ICollection<Employee> GroupMembers { get; set; } = new List<Employee>();

    }
}
