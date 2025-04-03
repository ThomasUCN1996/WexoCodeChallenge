using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class MovieResponseDTO
    {

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        public List<Movie> Results { get; set; }
    }
}
