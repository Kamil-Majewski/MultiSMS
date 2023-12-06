namespace MultiSMS.Interface.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public int PhoneNumber { get; set; }
        public string? Department { get; set; }
        public int? DepartmentNumber { get; set; }
        public string? HQAddress { get; set; }
        public EmployeesRole EmployeeRole { get; set; } = default!;
    }
}
