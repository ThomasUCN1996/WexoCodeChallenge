using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class Video
    {
        [JsonProperty("iso_639_1")]
        public string LanguageCode { get; set; }

        [JsonProperty("iso_3166_1")]
        public string CountryCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("official")]
        public bool Official { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
