using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class Credits
    {
        [JsonProperty("cast")]
        public List<Cast> Cast { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }
    }
}
