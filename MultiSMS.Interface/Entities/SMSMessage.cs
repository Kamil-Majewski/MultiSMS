namespace MultiSMS.Interface.Entities
{
    public class SMSMessage
    {
        public int ChosenGroupId { get; set; }
        public string? AdditionalPhoneNumbers { get; set; } 
        public DateTime MessageSentDate { get; set; } = DateTime.Now;
        public string? AdditionalInformation { get; set; }
        public Dictionary<string, object> Settings { get; set; } = default!;
        public object ServerResponse { get; set; } = default!;
    }
}
