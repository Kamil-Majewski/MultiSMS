using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.Comverga___MProfi
{
    public class ComvergaErrorResponse
    {
        [JsonProperty(PropertyName = "detail", Required = Required.Always)]
        public string Detail { get; set; } = default!;
    }
}
