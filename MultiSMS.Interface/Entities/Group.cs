using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiSMS.Interface.Entities
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public string? GroupDescription { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public ICollection<EmployeeGroup>? GroupMembers { get; set; }
        [NotMapped]
        public ICollection<int> MembersIds { get; set;} = new List<int>();
        [NotMapped]
        public ICollection<Employee>? Members { get; set; }

    }
}
