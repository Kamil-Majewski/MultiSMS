namespace MultiSMS.BusinessLogic.DTO
{
    public class SmsMessageDTO
    {
        public int SMSId { get; set; }
        public int IssuerId { get; set; }
        public int ChosenGroupId { get; set; }
        public string? AdditionalPhoneNumbers { get; set; }
        public DateTime MessageSentDate { get; set; }
        public string? AdditionalInformation { get; set; }
        public Dictionary<string, string> DataDictionary { get; set; } = default!;
        public object ServerResponse { get; set; } = default!;
    }
}
