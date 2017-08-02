using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeneralBot.Models.Urban
{
    public class UrbanResponse
    {
        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("list")]
        public IList<Result> Results { get; set; }

        [JsonProperty("sounds")]
        public IList<string> Sounds { get; set; }
    }
}