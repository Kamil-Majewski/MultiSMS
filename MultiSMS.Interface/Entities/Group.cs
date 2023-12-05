namespace MultiSMS.Interface.Entities
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public ICollection<User>? Users { get; set; }

    }
}
