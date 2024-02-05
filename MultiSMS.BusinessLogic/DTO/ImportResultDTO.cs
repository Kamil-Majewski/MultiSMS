using MultiSMS.Interface.Entities;

namespace MultiSMS.BusinessLogic.DTO
{
    public class ImportResultDTO
    {
        public int ImportId { get; set; }
        public string ImportStatus { get; set; } = default!;
        public string ImportMessage { get; set; } = default!;
        public List<Employee>? AddedEmployees { get; set; }
        public List<Employee>? RepeatedEmployees { get; set; }
        public List<Employee>? InvalidEmployees { get; set; }
        public List<int>? NonExistantGroupIds { get; set; }
    }
}
