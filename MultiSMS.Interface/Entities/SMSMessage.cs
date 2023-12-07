using System.Text.RegularExpressions;

namespace MultiSMS.Interface.Entities
{
    public class SMSMessage
    {
        public int SMSId { get; set; }
        public string Issuer { get; set; } = default!;
        public string SMSContent { get; set; } = default!;
        public Group ChosenGroup { get; set; } = default!;
        public int DutyOfficersPhoneNumber { get; set; }
        public ICollection<int> AdditionalPhoneNumbers { get; set; } = new List<int>();
        public DateTime MessageSentDate { get; set; } = DateTime.Now;

    }
}
