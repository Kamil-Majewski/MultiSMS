namespace MultiSMS.Interface.Entities
{
    public class ApiSmsSender
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string ApiProvider { get; set; } = default!;
        public string? ApiToken { get; set; }

        public List<int> AssingedUserIds { get; set; } = new();
    }
}
