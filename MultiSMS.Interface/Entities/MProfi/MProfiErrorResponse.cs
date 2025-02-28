using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.MProfi
{
    public class MProfiErrorResponse
    {
        [JsonProperty(PropertyName = "detail", Required = Required.Always)]
        public string Detail { get; set; } = default!;
    }
}
