using System.Runtime.CompilerServices;

namespace MultiSMS.Interface.Entities
{
    public class ImportResult
    {
        public string ImportStatus { get; set; } = default!;
        public string ImportMessage { get; set; } = default!;
        public IEnumerable<Employee>? AddedEmployees { get; set; }
        public IEnumerable<Employee>? RepeatedEmployees { get; set; }
        public IEnumerable<Employee>? InvalidEmployees { get; set; }
        public IEnumerable<IEnumerable<string>>? NonExistantGroupIds { get; set; }
    }
}