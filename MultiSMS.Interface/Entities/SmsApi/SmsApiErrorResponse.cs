using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.SmsApi
{
    public class SmsApiErrorResponse
    {
        [JsonProperty(PropertyName = "invalid_numbers", Required = Required.Default)]
        public List<InvalidNumber>? InvalidNumbers { get; set; } = new();

        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public string ErrorCode { get; set; } = default!;

        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string ErrorMessage { get; set; } = default!;

        [JsonProperty(PropertyName = "errors", Required = Required.Default)]
        public List<Error> Errors { get; set; } = new();
    }

    public class InvalidNumber
    {
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        public string Number { get; set; } = default!;

        [JsonProperty(PropertyName = "submitted_number", Required = Required.Always)]
        public string SubmittedNumber = default!;

        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; } = default!;
    }

    public class Error
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public string ErrorType { get; set; } = default!;
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string ErrorMessage { get; set; } = default!;
    }
}
