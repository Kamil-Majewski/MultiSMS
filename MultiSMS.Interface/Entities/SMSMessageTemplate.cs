using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("TemplateId")]
    public class SMSMessageTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = default!;
        public string? TemplateDescription { get; set; }
        public string TemplateContent { get; set; } = default!;
    }
}
