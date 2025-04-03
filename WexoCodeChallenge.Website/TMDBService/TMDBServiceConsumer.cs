using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WexoCodeChallenge.Website.TMDBService.DTO;

namespace WexoCodeChallenge.Website.TMDBService
{
    public class TMDBServiceConsumer
    {
        private const string ApiBaseUrl = "https://api.themoviedb.org/3/";
        private const string AuthorizationToken = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MTUxMWVlMjI5ZmYxMThmNTE1ODExNDI0MGJjODI5MCIsIm5iZiI6MTc0MjkwNDM1Mi43MTcsInN1YiI6IjY3ZTI5YzIwZDcwYzYxNTkwMzc1ZTY1NSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.fR2w_9rTwaEWFervhrhI5mbcHun7q4Ww2NNvPXELHMs";
        private readonly RestClient _restClient;

        public TMDBServiceConsumer()
        {
            _restClient = new RestClient(new RestClientOptions(ApiBaseUrl));
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            var request = CreateRequest("genre/movie/list", Method.Get);
            var response = await ExecuteRequestAsync<GenreResponseDTO>(request);
            return response?.Genres ?? new List<Genre>();
        }

        public async Task<int> GetMovieCountByGenreAsync(int genreId)
        {
            var request = CreateRequest("discover/movie", Method.Get);
            request.AddParameter("with_genres", genreId);
            var response = await ExecuteRequestAsync<MovieResponseDTO>(request);
            return response?.TotalResults ?? 0;
        }

        public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId, int page = 1)
        {
            var request = CreateRequest("discover/movie", Method.Get);
            request.AddParameter("with_genres", genreId);
            request.AddParameter("sort_by", "popularity.desc");
            request.AddParameter("page", page);
            var response = await ExecuteRequestAsync<MovieResponseDTO>(request);


            if (response?.Results != null)
            {
                string baseImageUrl = "https://image.tmdb.org/t/p/w500";
                foreach (var movie in response.Results)
                {
                    movie.PosterPath = !string.IsNullOrEmpty(movie.PosterPath)
                        ? $"{baseImageUrl}{movie.PosterPath}"
                        : "/path/to/placeholder.jpg"; // Fallback image
                }
            }



            return response?.Results ?? new List<Movie>();
        }

        public async Task<Movie> GetMovieDetailsAsync(int movieId)
        {
            
            var request = CreateRequest($"movie/{movieId}", Method.Get);
            var movieDetails = await ExecuteRequestAsync<Movie>(request);

            if (movieDetails != null && !string.IsNullOrEmpty(movieDetails.PosterPath)) {
                const string baseImageUrl = "https://image.tmdb.org/t/p/w500";
                movieDetails.PosterPath = $"{baseImageUrl}{movieDetails.PosterPath}";

                movieDetails.BackdropPath = !string.IsNullOrEmpty(movieDetails.BackdropPath)
                ? $"{baseImageUrl}{movieDetails.BackdropPath}"
                : "/path/to/placeholder.jpg"; // Fallback image

                movieDetails.Videos = await GetMovieVideosAsync(movieId);
            }

            return movieDetails;
        }

        private RestRequest CreateRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", AuthorizationToken);
            return request;
        }

        private async Task<T> ExecuteRequestAsync<T>(RestRequest request) where T : class
        {
            try
            {
                var response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Invalid API token.");
                }

                if (!response.IsSuccessful)
                {
                    throw new Exception($"API request failed with status code {response.StatusCode}.");
                }

                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new Exception($"An error occurred while processing the request: {ex.Message}", ex);
            }
        }

        private async Task<List<Video>> GetMovieVideosAsync(int movieId)
        {
            var request = CreateRequest($"movie/{movieId}/videos", Method.Get);
            var response = await ExecuteRequestAsync<VideoReponseDTO>(request);
            return response?.Results
                .Where(v => v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase) && v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                .ToList() ?? new List<Video>();
        }
        
    }
}
