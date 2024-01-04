using System.ComponentModel.DataAnnotations.Schema;

namespace MultiSMS.Interface.Entities
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public string? GroupDescription { get; set; }
        public ICollection<EmployeeGroup>? GroupMembers { get; set; }
        [NotMapped]
        public ICollection<int> MembersIds { get; set;} = new List<int>();

    }
}
