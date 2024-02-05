using Microsoft.EntityFrameworkCore;

namespace MultiSMS.Interface.Entities
{
    [PrimaryKey("ImportId")]
    public class ImportResult
    {
        public int ImportId { get; set; }
        public string ImportStatus { get; set; } = default!;
        public string ImportMessage { get; set; } = default!;
        public string? AddedEmployeesSerialized { get; set; }
        public string? RepeatedEmployeesSerialized { get; set; }
        public string? InvalidEmployeesSerialized { get; set; }
        public string? NonExistantGroupIdsSerialized { get; set; }
    }
}