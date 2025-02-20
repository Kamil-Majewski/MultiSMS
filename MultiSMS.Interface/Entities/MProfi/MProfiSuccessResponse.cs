﻿using Newtonsoft.Json;

namespace MultiSMS.Interface.Entities.MProfi
{
    public class MProfiSuccessResponse
    {
        [JsonProperty(PropertyName = "result", Required = Required.Always)]
        public List<ResultItem> Result { get; set; } = new();
    }

    public class ResultItem
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; } = default!;
    }
}
