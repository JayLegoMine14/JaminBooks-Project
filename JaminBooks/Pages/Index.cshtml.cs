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
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages
{
    public class IndexModel : PageModel
    {
        public User CurrentUser;
        public Book Reccomended;
        public Book MostPopular;
        public Book BestSeller;

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            Reccomended = BookPreviews.GetReccomended(CurrentUser);
            MostPopular = BookPreviews.GetMostPopular();
            BestSeller = BookPreviews.GetBestSeller();
        }
    }
}
