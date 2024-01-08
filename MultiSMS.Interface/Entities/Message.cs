namespace MultiSMS.Interface.Entities
{
    public class Message
    {
        public ICollection<string> To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string MessageContent { get; set; } = default!;
    }
}
