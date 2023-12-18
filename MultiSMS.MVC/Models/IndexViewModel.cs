using MultiSMS.Interface.Entities;

namespace MultiSMS.MVC.Models
{
    public class IndexViewModel
    {
        public Administrator Administrator { get; set; } = new Administrator();
        public Employee Employee { get; set; } = new Employee();
        public EmployeesGroup EmployeesGroup { get; set; } = new EmployeesGroup();
        public EmployeesRole EmployeesRole { get; set; } = new EmployeesRole();
        public PhoneNumber PhoneNumber { get; set; } = new PhoneNumber();
        public SMSMessage SMSMessage { get; set; } = new SMSMessage();
        public SMSMessageTemplate SMSMessageTemplate { get; set; } = new SMSMessageTemplate();
    }
}
