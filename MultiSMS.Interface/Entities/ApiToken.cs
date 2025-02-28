namespace MultiSMS.Interface.Entities
{
    public class ApiToken
    {
        public int Id { get; set; }
        public string Provider { get; set; } = default!;
        public string? Description { get; set; }
        public string Value { get; set; } = default!;
    }
}
