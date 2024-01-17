namespace MultiSMS.Interface.Entities
{
    public class ErrorResponse
    {
        public ErrorDetails Error { get; set; } = default!;
    }

    public class ErrorDetails
    {
        public int Code { get; set; }
        public string Type { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
