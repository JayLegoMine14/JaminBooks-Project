using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Summarizes the origin of the site and gives a brief biography of the creators.
    /// </summary>
    public class AboutModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
        }
    }
}