namespace MultiSMS.Interface.Entities
{
    public class ApiSmsSender
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public int ApiTokenId { get; set; }
        public ApiToken ApiToken { get; set; } = default!;

        public List<ApiSmsSenderUser> AssignedUsers { get; set; } = new();
    }
}
