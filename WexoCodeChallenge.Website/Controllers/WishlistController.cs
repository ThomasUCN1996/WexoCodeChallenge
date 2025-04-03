using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WexoCodeChallenge.Website.TMDBService;
using WexoCodeChallenge.Website.TMDBService.DTO;

namespace WexoCodeChallenge.Website.Controllers
{
    public class WishlistController : Controller
    {


        private readonly ILogger<WishlistController> _logger;
        private readonly TMDBServiceConsumer _tmdbServiceConsumer;
        private const string WishlistCookieName = "Wishlist";


        public WishlistController(ILogger<WishlistController> logger)
        {
            _logger = logger;
            _tmdbServiceConsumer = new TMDBServiceConsumer();
        }

        public IActionResult Index()
        {
            var wishlist = GetWishlistFromCookie();
            return View(wishlist);
        }


        public IActionResult AddToWishlist(int id, string title, string posterPath)
        {
            var wishlist = GetWishlistFromCookie();

            if(!wishlist.Exists(m => m.Id == id))
            {
                wishlist.Add(new Movie
                {
                    Id = id,
                    Title = title,
                    PosterPath = posterPath
                });
                SaveWishlistToCookie(wishlist);

            }
            return RedirectToAction("Index", "Wishlist");
        }

        

        public IActionResult RemoveFromWishlist(int id)
        {
            var wishlist = GetWishlistFromCookie();
            wishlist.RemoveAll(m => m.Id == id);
            SaveWishlistToCookie(wishlist);
            return RedirectToAction("Index", "Wishlist");
        }

        private List<Movie> GetWishlistFromCookie()
        {
            var wishlistCookie = Request.Cookies[WishlistCookieName];

            if (string.IsNullOrEmpty(wishlistCookie))
            {
                return new List<Movie>(); // Return an empty list if no cookie exists
            }

            try
            {
                return JsonConvert.DeserializeObject<List<Movie>>(wishlistCookie) ?? new List<Movie>();
            }
            catch (JsonException)
            {
                return new List<Movie>(); // If JSON is invalid, return an empty list
            }
        }

        private void SaveWishlistToCookie(List<Movie> wishlist)
        {
            var wishlistJson = JsonConvert.SerializeObject(wishlist);
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30), // Set cookie expiration (optional)
                HttpOnly = true, // Make the cookie HttpOnly for security
                Secure = true, // Only send the cookie over HTTPS (optional, but recommended)
                SameSite = SameSiteMode.Strict // Set the SameSite attribute (optional, for cross-site cookie security)
            };

            Response.Cookies.Append(WishlistCookieName, wishlistJson, options);
        }


    }
}
