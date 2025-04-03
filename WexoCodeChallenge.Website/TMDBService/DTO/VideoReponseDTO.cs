using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class VideoReponseDTO
    {
        [JsonProperty("results")]
        public List<Video> Results { get; set; }

    }
}
