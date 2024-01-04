namespace MultiSMS.Interface.Entities
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogType { get; set; } = default!;
        public string LogSource { get; set; } = default!;
        public string LogMessage { get; set; } = default!;
        public DateTime LogCreated { get; set; } = DateTime.UtcNow;
        public int? LogRelatedObjectId { get; set; } = default!;
    }
}
