using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities
{
    public class SuccessResponse
    {
        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "queued", Required = Required.Always)]
        public int Queued { get; set; }
        [JsonProperty(PropertyName = "unsent", Required = Required.Always)]
        public int Unsent { get; set; }
        [JsonProperty(PropertyName = "items", Required = Required.AllowNull)]
        public Item[] Items { get; set; } = default!;
    }

    public class Item
    {
        public string Id { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Queued { get; set; } = default!;
        public int Parts { get; set; }
        public string Text { get; set; } = default!;
        public int Stat_id { get; set; } = default!;
    }
}
