using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class Cast
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
