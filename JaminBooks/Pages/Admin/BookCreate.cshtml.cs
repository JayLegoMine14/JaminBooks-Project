using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for creating or editing a book.
    /// </summary>
    public class BookCreateModel : PageModel
    {
        /// <summary>
        /// The book to display.
        /// </summary>
        public Book Book;

        /// <summary>
        /// A list of all publishers.
        /// </summary>
        public List<Publisher> Publishers = Publisher.GetPublishers();

        /// <summary>
        /// A list of all authors.
        /// </summary>
        public List<Author> Authors = Author.GetAuthors();

        /// <summary>
        /// A list of all categories.
        /// </summary>
        public List<Category> Categories = Category.GetCategories();

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="id">The id number of the book to load. (optional)</param>
        public void OnGet(int? id)
        {
            User user = Authentication.GetCurrentUser(HttpContext);
            if (user == null || !user.IsAdmin) Response.Redirect("/");
            Book = (id == null ? null : new Book(id.Value));
        }
    }
}