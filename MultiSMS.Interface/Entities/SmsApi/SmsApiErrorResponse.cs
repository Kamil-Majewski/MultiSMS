using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.SmsApi
{
    public class SmsApiErrorResponse
    {
        [JsonProperty(PropertyName = "invalid_numbers", Required = Required.Always)]
        public List<InvalidNumber> InvalidNumbers { get; set; } = new List<InvalidNumber>();

        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string ErrorMessage { get; set; } = default!;
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
}
