using Newtonsoft.Json;

namespace PRIME.Core.Common.Models.CDS
{
    public class CardSource
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}