using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("EmployeeId", "GroupId")]
    public class EmployeeGroup
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
        
        public int GroupId { get; set; }
        public Group Group { get; set; } = default!;


    }
}
