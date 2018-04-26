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
    public class BannerModel : PageModel
    {
        public User CurrentUser;
        public List<Banner> Banners;

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