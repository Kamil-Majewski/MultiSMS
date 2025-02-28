namespace MultiSMS.Interface.Entities
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogType { get; set; } = default!;
        public string LogSource { get; set; } = default!;
        public string LogMessage { get; set; } = default!;
        public string LogCreator { get; set; } = default!;
        public int LogCreatorId { get; set; }
        public DateTime LogCreationDate { get; set; } = DateTime.UtcNow;
        public string LogRelatedObjectsDictionarySerialized { get; set; } = default!;
    }
}
