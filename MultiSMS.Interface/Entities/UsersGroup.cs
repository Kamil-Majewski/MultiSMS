namespace MultiSMS.Interface.Entities
{
    public class UsersGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public string? GroupDescription { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
