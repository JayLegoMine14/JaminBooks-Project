using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Pages
{
    public class IndexModel : PageModel
    {
        public User CurrentUser;
        public Book Reccomended;
        public List<Banner> Banners;
        public List<Book> MostPopular;
        public List<Book> BestSellers;
        public List<Book> OnSale;

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            Reccomended = BookPreviews.GetReccomended(CurrentUser);
            MostPopular = BookPreviews.GetMostPopular();
            BestSellers = BookPreviews.GetBestSellers();
            OnSale = BookPreviews.GetSales();
            Banners = Banner.GetBanners();
        }
    }
}
