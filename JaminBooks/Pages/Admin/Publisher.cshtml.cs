using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for creating or editing a publisher.
    /// </summary>
    public class PublisherModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The publisher to display.
        /// </summary>
        public Publisher Publisher;

        /// <summary>
        /// A list of phone categories.
        /// </summary>
        public Dictionary<int, string> Categories;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="id">The id number of the publisher to load. (optional)</param>
        public void OnGet(int? id)
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            Publisher = id != null ? new Publisher(id.Value) : null;
            Categories = Phone.GetPhoneCategories();
        }
    }
}