using Newtonsoft.Json;

namespace WexoCodeChallenge.Website.TMDBService.DTO
{
    public class Genre
    {


        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public int MovieCount { get; set; }

        public List<Movie> Movies { get; set; } // List of movies for each genre
    }
}
