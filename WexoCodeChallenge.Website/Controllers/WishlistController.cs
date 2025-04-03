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
        private const string WishlistCookieName = "Wishlist"; // Cookie name for storing wishlist data

        public WishlistController(ILogger<WishlistController> logger)
        {
            _logger = logger;
            _tmdbServiceConsumer = new TMDBServiceConsumer(); // Initialize the TMDBServiceConsumer
        }

        /// <summary>
        /// Displays the current wishlist.
        /// </summary>
        /// <returns>Returns the view with the wishlist</returns>
        public IActionResult Index()
        {
            // Get the wishlist from the cookies
            var wishlist = GetWishlistFromCookie();

            // Return the wishlist to the view
            return View(wishlist);
        }

        /// <summary>
        /// Adds a movie to the wishlist.
        /// </summary>
        /// <param name="id">The movie's ID</param>
        /// <param name="title">The movie's title</param>
        /// <param name="posterPath">The movie's poster path</param>
        /// <returns>Redirects back to the wishlist index</returns>
        public IActionResult AddToWishlist(int id, string title, string posterPath)
        {
            // Get the current wishlist from the cookies
            var wishlist = GetWishlistFromCookie();

            // Check if the movie is already in the wishlist
            if (!wishlist.Exists(m => m.Id == id))
            {
                // Add the movie to the wishlist if it's not already there
                wishlist.Add(new Movie
                {
                    Id = id,
                    Title = title,
                    PosterPath = posterPath
                });

                // Save the updated wishlist back to the cookies
                SaveWishlistToCookie(wishlist);
            }

            // Redirect to the wishlist index page
            return RedirectToAction("Index", "Wishlist");
        }

        /// <summary>
        /// Removes a movie from the wishlist.
        /// </summary>
        /// <param name="id">The movie's ID to remove</param>
        /// <returns>Redirects back to the wishlist index</returns>
        public IActionResult RemoveFromWishlist(int id)
        {
            // Get the current wishlist from the cookies
            var wishlist = GetWishlistFromCookie();

            // Remove the movie with the specified ID from the wishlist
            wishlist.RemoveAll(m => m.Id == id);

            // Save the updated wishlist back to the cookies
            SaveWishlistToCookie(wishlist);

            // Redirect to the wishlist index page
            return RedirectToAction("Index", "Wishlist");
        }

        /// <summary>
        /// Retrieves the wishlist stored in the cookie.
        /// </summary>
        /// <returns>Returns a list of movies from the wishlist stored in the cookie</returns>
        private List<Movie> GetWishlistFromCookie()
        {
            // Retrieve the wishlist cookie by name
            var wishlistCookie = Request.Cookies[WishlistCookieName];

            // If the cookie doesn't exist or is empty, return an empty list
            if (string.IsNullOrEmpty(wishlistCookie))
            {
                return new List<Movie>();
            }

            try
            {
                // Attempt to deserialize the JSON string into a list of movies
                return JsonConvert.DeserializeObject<List<Movie>>(wishlistCookie) ?? new List<Movie>();
            }
            catch (JsonException)
            {
                // If there's an issue with deserialization (invalid JSON), return an empty list
                return new List<Movie>();
            }
        }

        /// <summary>
        /// Saves the wishlist to the cookie.
        /// </summary>
        /// <param name="wishlist">The wishlist to save</param>
        private void SaveWishlistToCookie(List<Movie> wishlist)
        {
            // Serialize the wishlist to a JSON string
            var wishlistJson = JsonConvert.SerializeObject(wishlist);

            // Set cookie options
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5), // Set the expiration time for the cookie
                HttpOnly = true, // Make the cookie HttpOnly for added security
                Secure = true, // Send the cookie only over HTTPS (recommended for security)
                SameSite = SameSiteMode.Strict // Set the SameSite attribute for cross-site security
            };

            // Save the serialized wishlist JSON to the cookie
            Response.Cookies.Append(WishlistCookieName, wishlistJson, options);
        }
    }
}
