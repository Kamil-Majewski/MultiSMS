using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("EmployeeId")]
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string Email { get; set; } = default!;
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = default!;
        public string? Department { get; set; }
        public string? PostalNumber { get; set; }
        public string? City { get; set; }
        public string? HQAddress { get; set; }
        public bool IsActive { get; set; }
        public ICollection<EmployeesRole>? EmployeeRole { get; set; }
        public ICollection<EmployeesGroup>? EmployeesGroup { get; set; }
    }
}
