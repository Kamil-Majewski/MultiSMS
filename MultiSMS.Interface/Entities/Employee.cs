using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("EmployeeId")]
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public PhoneNumber PhoneNumber { get; set; } = default!;
        public string? Department { get; set; }
        public int? DepartmentNumber { get; set; }
        public string? HQAddress { get; set; }
        public bool IsActive { get; set; }
        public ICollection<EmployeesRole> EmployeeRole { get; set; } = default!;

        public int GroupId { get; set; }
        public EmployeesGroup EmployeesGroup { get; set; } = default!;
    }
}
