using Newtonsoft.Json;

namespace CNNDesktop.Models
{
    public class News
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}