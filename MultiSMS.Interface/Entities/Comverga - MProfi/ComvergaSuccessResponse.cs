using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.Comverga___MProfi
{
    public class ComvergaSuccessResponse
    {
        [JsonProperty(PropertyName = "result", Required = Required.Always)]
        public List<ResultItem> Result { get; set; } = new();
    }

    public class ResultItem
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }
    }
}
