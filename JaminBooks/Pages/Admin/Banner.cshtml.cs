using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for creating, deleting, and ordering banners.
    /// </summary>
    public class BannerModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// A list of banners to display.
        /// </summary>
        public List<Banner> Banners;

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

            Banners = Banner.GetBanners();
        }
    }
}