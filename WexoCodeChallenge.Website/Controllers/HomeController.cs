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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _tmdbServiceConsumer = new TMDBServiceConsumer();

        }

        public async Task<IActionResult> Index()
        {

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

            var genres = await _tmdbServiceConsumer.GetGenresAsync();

            var filteredGenres = genres.Where(g => desiredGenreIds.Contains(g.Id)).ToList();

            foreach (var genre in filteredGenres)
            {
                genre.MovieCount = await _tmdbServiceConsumer.GetMovieCountByGenreAsync(genre.Id);
                genre.Movies = await _tmdbServiceConsumer.GetMoviesByGenreAsync(genre.Id, 1);
            }
            

            return View(filteredGenres);
        }


        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Details(int id, int page = 1)
        {
            const int pageSize = 10;
            var genres = await _tmdbServiceConsumer.GetGenresAsync();
            var genre = genres.FirstOrDefault(g => g.Id == id);

            if (genre == null)
            {
                return NotFound();
            }
            genre.Movies = await _tmdbServiceConsumer.GetMoviesByGenreAsync(id, page) ?? new List<Movie>();

            var totalMovies = await _tmdbServiceConsumer.GetMovieCountByGenreAsync(id);
            var totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);


            ViewBag.currentPage = page;
            ViewBag.totalPages = totalPages;
            ViewBag.genreName = genre.Name;
            

            return View(genre);
        }

        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _tmdbServiceConsumer.GetMovieDetailsAsync(id);
            if(movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }


    }
}
