using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.MProfi
{
    public class MProfiSuccessResponse
    {
        [JsonProperty(PropertyName = "result", Required = Required.Always)]
        public List<ResultItem> Result { get; set; } = new();
    }

    public class ResultItem
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; } = default!;

        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }

        [JsonProperty("error_message")]
        public string? ErrorMessage { get; set; }
    }
}
