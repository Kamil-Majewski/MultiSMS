
using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("SMSId")]
    public class SMSMessage
    {
        public int SMSId { get; set; }
        public string Issuer { get; set; } = default!;
        public string SMSContent { get; set; } = default!;
        public EmployeesGroup ChosenGroup { get; set; } = default!;
        public List<Employee> AdditionalEmployees { get; set; } = new List<Employee>();
        public DateTime MessageSentDate { get; set; } = DateTime.Now;
    }
}
