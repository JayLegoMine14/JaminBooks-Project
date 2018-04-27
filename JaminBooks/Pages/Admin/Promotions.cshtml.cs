using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for creating, editing, and deleting promotions.
    /// </summary>
    public class PromotionsModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// A list of promotions on order totals.
        /// </summary>
        public List<Promotion> TotalPromotions;

        /// <summary>
        /// A list of promotions using promotion codes.
        /// </summary>
        public List<Promotion> CodePromotions;

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

            List<Promotion> all = Promotion.GetPromotions();
            TotalPromotions = all.Where(p => p.Total != null).ToList();
            TotalPromotions.Sort((a, b) => (b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now).CompareTo((a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)));
            CodePromotions = all.Where(p => p.Code != null).ToList();
            CodePromotions.Sort((a, b) => (b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now).CompareTo((a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)));
        }
    }
}