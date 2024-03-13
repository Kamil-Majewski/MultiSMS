using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.SmsApi
{
    public class SmsApiSuccessResponse
    {
       [JsonProperty(PropertyName = "count", Required = Required.Always)]
       public int SentCount { get; set; }

        [JsonProperty(PropertyName = "list", Required = Required.Always)]
        public List<SuccessDetail> Details { get; set; } = new List<SuccessDetail>();

    }

    public class SuccessDetail
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "points", Required = Required.Always)]
        public float Points { get; set; }

        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        public string PhoneNumber { get; set; } = default!;

        [JsonProperty(PropertyName = "date_sent", Required = Required.Always)]
        public long DateSent { get; set; }

        [JsonProperty(PropertyName = "submitted_number", Required = Required.Always)]
        public string SubmittedNumber { get; set; } = default!;

        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public string Status { get; set; } = default!;
    }
}
