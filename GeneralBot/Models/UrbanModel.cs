using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralBot.Models
{
    public class Result
    {

        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("permalink")]
        public string PermaLink { get; set; }

        [JsonProperty("thumbs_up")]
        public int Likes { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("defid")]
        public int Id { get; set; }

        [JsonProperty("current_vote")]
        public string CurrentVote { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_down")]
        public int Dislikes { get; set; }
    }

    public class UrbanModel
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
