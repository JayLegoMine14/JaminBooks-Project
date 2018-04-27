using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Displays the data of a book and allows for the creation of ratings.
    /// </summary>
    public class BookModel : PageModel
    {
        /// <summary>
        /// The book being displayed.
        /// </summary>
        public Book Book;

        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The book's current promotion.
        /// </summary>
        public Promotion Promo;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="id">The id number of the book to load.</param>
        public void OnGet(int id)
        {
            Promotion.DeleteExpiredPromotions(id);
            Book = new Book(id);
            Promo = Promotions.GetPromotion(Book);
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
        }
    }
}