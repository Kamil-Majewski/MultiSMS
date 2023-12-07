
namespace MultiSMS.Interface.Entities
{
    public class PhoneNumber
    {
        public int PhoneNumberId { get; set; }
        public string Number { get; set; } = default!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
    }
}
