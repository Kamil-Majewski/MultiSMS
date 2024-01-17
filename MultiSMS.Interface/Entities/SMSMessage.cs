
using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("SMSId")]
    public class SMSMessage
    {
        public int SMSId { get; set; }
        public int IssuerId { get; set; }
        public int ChosenGroupId { get; set; }
        public List<int> AdditionalEmployeesIds { get; set; } = new List<int>();
        public DateTime MessageSentDate { get; set; } = DateTime.Now;
        public string? AdditionalInformation { get; set; }
        public string DataDictionarySerialized { get; set; } = default!;
        public string ServerResponseSerialized { get; set; } = default!;
    }
}
