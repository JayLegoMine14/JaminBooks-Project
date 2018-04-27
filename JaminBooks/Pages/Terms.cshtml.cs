using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    /// <summary>
    /// The site's terms of service.
    /// </summary>
    public class TermsModel : PageModel
    {
        /// <summary>
        /// Load the page on a get request.
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