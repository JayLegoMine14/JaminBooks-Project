using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages
{
    /// <summary>
    /// The site's home page which displays banners and lists of books on sale, best sellers, and most popular.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The banners to display on the home page.
        /// </summary>
        public List<Banner> Banners;

        /// <summary>
        /// Most popular books based on ratings and comments.
        /// </summary>
        public List<Book> MostPopular;

        /// <summary>
        /// Best sellers based on number of sales.
        /// </summary>
        public List<Book> BestSellers;

        /// <summary>
        /// Books that are currently on sale.
        /// </summary>
        public List<Book> OnSale;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            MostPopular = BookPreviews.GetMostPopular();
            BestSellers = BookPreviews.GetBestSellers();
            OnSale = BookPreviews.GetSales();
            Banners = Banner.GetBanners();
        }
    }
}