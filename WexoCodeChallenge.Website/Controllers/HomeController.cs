using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WexoCodeChallenge.Website.Models;
using WexoCodeChallenge.Website.TMDBService;
using WexoCodeChallenge.Website.TMDBService.DTO;

namespace WexoCodeChallenge.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TMDBServiceConsumer _tmdbServiceConsumer;

        // Constructor to initialize the logger and TMDB service consumer
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _tmdbServiceConsumer = new TMDBServiceConsumer();
        }

        /// <summary>
        /// Displays a list of selected genres along with movie counts and movie lists.
        /// </summary>
        /// <returns>Returns a view with the filtered genres and associated movies</returns>
        public async Task<IActionResult> Index()
        {
            // List of desired genre IDs to fetch
            var desiredGenreIds = new List<int>
            {
                28,  // Action
                35,  // Comedy
                53,  // Thriller
                10752, // War
                10749, // Romance
                18,   // Drama
                80,   // Crime
                99,   // Documentary
                27    // Horror
            };

            // Get all genres from the TMDB service
            var genres = await _tmdbServiceConsumer.GetGenresAsync();

            // Filter the genres based on the desired genre IDs
            var filteredGenres = genres.Where(g => desiredGenreIds.Contains(g.Id)).ToList();

            // For each filtered genre, fetch the movie count and list of movies
            foreach (var genre in filteredGenres)
            {
                genre.MovieCount = await _tmdbServiceConsumer.GetMovieCountByGenreAsync(genre.Id);
                genre.Movies = await _tmdbServiceConsumer.GetMoviesByGenreAsync(genre.Id, 1);
            }

            // Return the filtered genres to the view for display
            return View(filteredGenres);
        }

        /// <summary>
        /// Displays the Privacy Policy page.
        /// </summary>
        /// <returns>Returns the Privacy view</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays an error page when an exception occurs.
        /// </summary>
        /// <returns>Returns the error page with the request ID</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Displays the list of movies for a specific genre with pagination.
        /// </summary>
        /// <param name="id">The genre ID</param>
        /// <param name="page">The current page number (defaults to 1)</param>
        /// <returns>Returns the view with the selected genre and movie list</returns>
        public async Task<IActionResult> Details(int id, int page = 1)
        {
            const int pageSize = 10; // The number of movies to display per page

            // Get all available genres
            var genres = await _tmdbServiceConsumer.GetGenresAsync();

            // Find the genre based on the ID passed in the request
            var genre = genres.FirstOrDefault(g => g.Id == id);

            // If the genre doesn't exist, return a 404 Not Found error
            if (genre == null)
            {
                return NotFound();
            }

            // Fetch movies for the selected genre and the current page
            genre.Movies = await _tmdbServiceConsumer.GetMoviesByGenreAsync(id, page) ?? new List<Movie>();

            // Calculate the total number of pages based on the total movie count for the genre
            var totalMovies = await _tmdbServiceConsumer.GetMovieCountByGenreAsync(id);
            var totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            // Pass pagination information to the view
            ViewBag.currentPage = page;
            ViewBag.totalPages = totalPages;
            ViewBag.genreName = genre.Name;

            // Return the genre details and movie list to the view
            return View(genre);
        }

        /// <summary>
        /// Displays detailed information for a specific movie.
        /// </summary>
        /// <param name="id">The movie ID</param>
        /// <returns>Returns the view with the movie details</returns>
        public async Task<IActionResult> MovieDetails(int id)
        {
            // Fetch movie details using the TMDB service
            var movie = await _tmdbServiceConsumer.GetMovieDetailsAsync(id);

            // If the movie does not exist, return a 404 Not Found error
            if (movie == null)
            {
                return NotFound();
            }

            // Return the movie details to the view for display
            return View(movie);
        }
    }
}
