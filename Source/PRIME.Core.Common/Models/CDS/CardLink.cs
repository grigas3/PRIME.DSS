using Newtonsoft.Json;

namespace PRIME.Core.Common.Models.CDS
{
    public class CardLink
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("url")]

        public string Url { get; set; }
        [JsonProperty("type")]
        public string UrlType { get; set; }

    }
}