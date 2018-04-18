using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages.Admin
{
    public class FlaggedRatingsModel : PageModel
    {
        public User CurrentUser;
        public List<Rating> Ratings;

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