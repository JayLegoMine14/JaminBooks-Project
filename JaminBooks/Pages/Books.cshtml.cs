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
        public string Message { get; set; }
        public User CurrentUser;

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);

            if (CurrentUser != null && CurrentUser.IsAdmin)
            {
                Message = "Welcome Admin to your application description page.";
            }
            else
            {
                Message = "Your application description page.";
            }
        }
    }
}
