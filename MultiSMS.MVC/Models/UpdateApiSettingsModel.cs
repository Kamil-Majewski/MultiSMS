namespace MultiSMS.MVC.Models
{
    public class UpdateApiSettingsModel
    {
        public string ActiveApiName { get; set; } = default!;
        public string SenderName { get; set; } = default!;
        public bool FastChannel { get; set; }
        public bool TestMode { get; set; }
    }
}
