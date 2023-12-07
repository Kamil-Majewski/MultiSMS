namespace MultiSMS.Interface.Entities
{
    public class SMSMessageTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = default!;
        public string SMSContent { get; set; } = default!;
    }
}
