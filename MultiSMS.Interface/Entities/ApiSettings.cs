using System.ComponentModel.DataAnnotations;

namespace MultiSMS.Interface.Entities
{
    public class ApiSettings
    {
        [Key]
        public int ApiSettingsId { get; set; }
        public string ApiName { get; set; } = default!;
        public bool ApiActive { get; set; } = default!;
        public bool FastChannel { get; set; }
        public bool TestMode { get; set; }
        public string SenderName { get; set; } = default!;

    }
}
