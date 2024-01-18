using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities
{
    public class ErrorResponse
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public ErrorDetails Error { get; set; } = default!;
    }

    public class ErrorDetails
    {
        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public int Code { get; set; }
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public string Type { get; set; } = default!;
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; } = default!;
    }
}
