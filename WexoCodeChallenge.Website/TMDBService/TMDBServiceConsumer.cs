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
        // Base URL for The Movie Database API
        private const string ApiBaseUrl = "https://api.themoviedb.org/3/";

        // Authorization token for authenticating API requests
        private const string AuthorizationToken = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MTUxMWVlMjI5ZmYxMThmNTE1ODExNDI0MGJjODI5MCIsIm5iZiI6MTc0MjkwNDM1Mi43MTcsInN1YiI6IjY3ZTI5YzIwZDcwYzYxNTkwMzc1ZTY1NSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.fR2w_9rTwaEWFervhrhI5mbcHun7q4Ww2NNvPXELHMs";

        // RestClient for making HTTP requests to the API
        private readonly RestClient _restClient;

        public TMDBServiceConsumer()
        {
            // Initialize RestClient with the base URL of the TMDB API
            _restClient = new RestClient(new RestClientOptions(ApiBaseUrl));
        }

        /// <summary>
        /// Retrieves a list of genres available in the TMDB API.
        /// </summary>
        /// <returns>List of genres</returns>
        public async Task<List<Genre>> GetGenresAsync()
        {
            // Create a GET request for the genre list endpoint
            var request = CreateRequest("genre/movie/list", Method.Get);

            // Execute the request and parse the response into GenreResponseDTO
            var response = await ExecuteRequestAsync<GenreResponseDTO>(request);

            // Return the list of genres or an empty list if no genres are found
            return response?.Genres ?? new List<Genre>();
        }

        /// <summary>
        /// Retrieves the total count of movies for a specific genre.
        /// </summary>
        /// <param name="genreId">The genre ID to filter movies by</param>
        /// <returns>Number of movies for the specified genre</returns>
        public async Task<int> GetMovieCountByGenreAsync(int genreId)
        {
            // Create a GET request to the discover movies endpoint with genre filter
            var request = CreateRequest("discover/movie", Method.Get);
            request.AddParameter("with_genres", genreId);

            // Execute the request and parse the response into MovieResponseDTO
            var response = await ExecuteRequestAsync<MovieResponseDTO>(request);

            // Return the total number of movies found for the genre, or 0 if no results
            return response?.TotalResults ?? 0;
        }

        /// <summary>
        /// Retrieves a list of movies for a specific genre with pagination.
        /// </summary>
        /// <param name="genreId">The genre ID to filter movies by</param>
        /// <param name="page">The page number to retrieve (default is 1)</param>
        /// <returns>List of movies for the specified genre</returns>
        public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId, int page = 1)
        {
            // Create a GET request to the discover movies endpoint with genre and page parameters
            var request = CreateRequest("discover/movie", Method.Get);
            request.AddParameter("with_genres", genreId);
            request.AddParameter("sort_by", "popularity.desc");
            request.AddParameter("page", page);

            // Execute the request and parse the response into MovieResponseDTO
            var response = await ExecuteRequestAsync<MovieResponseDTO>(request);

            // If movies are found, update the poster paths with full URLs
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

            // Return the list of movies or an empty list if no movies are found
            return response?.Results ?? new List<Movie>();
        }

        /// <summary>
        /// Retrieves detailed information for a specific movie by its ID.
        /// </summary>
        /// <param name="movieId">The movie ID to retrieve details for</param>
        /// <returns>Detailed movie information</returns>
        public async Task<Movie> GetMovieDetailsAsync(int movieId)
        {
            // Create a GET request for the movie details endpoint
            var request = CreateRequest($"movie/{movieId}", Method.Get);

            // Execute the request and parse the response into Movie
            var movieDetails = await ExecuteRequestAsync<Movie>(request);

            // If movie details are found, update image paths and fetch videos
            if (movieDetails != null && !string.IsNullOrEmpty(movieDetails.PosterPath))
            {
                const string baseImageUrl = "https://image.tmdb.org/t/p/w500";
                movieDetails.PosterPath = $"{baseImageUrl}{movieDetails.PosterPath}";

                movieDetails.BackdropPath = !string.IsNullOrEmpty(movieDetails.BackdropPath)
                    ? $"{baseImageUrl}{movieDetails.BackdropPath}"
                    : "/path/to/placeholder.jpg"; // Fallback image

                movieDetails.Videos = await GetMovieVideosAsync(movieId);
            }

            // Return the movie details
            return movieDetails;
        }

        /// <summary>
        /// Creates a new RestRequest with the necessary headers for TMDB API calls.
        /// </summary>
        /// <param name="resource">The resource endpoint (e.g., "genre/movie/list")</param>
        /// <param name="method">The HTTP method (e.g., GET, POST)</param>
        /// <returns>Configured RestRequest object</returns>
        private RestRequest CreateRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", AuthorizationToken);
            return request;
        }

        /// <summary>
        /// Executes an API request asynchronously and returns the parsed response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into</typeparam>
        /// <param name="request">The RestRequest to execute</param>
        /// <returns>Parsed response of type T</returns>
        private async Task<T> ExecuteRequestAsync<T>(RestRequest request) where T : class
        {
            try
            {
                // Execute the API request asynchronously
                var response = await _restClient.ExecuteAsync(request);

                // Handle unauthorized access
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Invalid API token.");
                }

                // Throw an exception if the response is not successful
                if (!response.IsSuccessful)
                {
                    throw new Exception($"API request failed with status code {response.StatusCode}.");
                }

                // Deserialize the response content into the specified type
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"An error occurred while processing the request: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves a list of video trailers for a specific movie.
        /// </summary>
        /// <param name="movieId">The movie ID to fetch videos for</param>
        /// <returns>List of video trailers (if any)</returns>
        private async Task<List<Video>> GetMovieVideosAsync(int movieId)
        {
            // Create a GET request for the movie videos endpoint
            var request = CreateRequest($"movie/{movieId}/videos", Method.Get);

            // Execute the request and parse the response into VideoReponseDTO
            var response = await ExecuteRequestAsync<VideoReponseDTO>(request);

            // Filter and return YouTube trailers only
            return response?.Results
                .Where(v => v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase) && v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                .ToList() ?? new List<Video>();
        }
    }
}
