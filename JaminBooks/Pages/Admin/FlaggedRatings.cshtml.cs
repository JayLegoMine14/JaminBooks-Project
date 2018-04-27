using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays a list of flagged ratings.
    /// </summary>
    public class FlaggedRatingsModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// Flagged ratings.
        /// </summary>
        public List<Rating> Ratings;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            Ratings = Rating.GetFlagged();
            Ratings.Sort((f1, f2) => f1.FlagUsers.Count.CompareTo(f2.FlagUsers.Count));
        }
    }
}