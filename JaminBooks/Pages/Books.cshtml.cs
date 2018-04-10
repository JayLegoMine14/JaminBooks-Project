using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    public class BooksModel : PageModel
    {
        public string Title { get; set; } = "";
        public User CurrentUser;

        public void OnGet(string title)
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (title != null) Title = title;
        }
    }
}
