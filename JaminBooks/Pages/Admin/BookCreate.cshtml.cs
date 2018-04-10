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
    public class BookCreateModel : PageModel
    {
        public Book Book;

        public void OnGet(int id)
        {
            User user = Authentication.GetCurrentUser(HttpContext);
            if (user == null || !user.IsAdmin) Response.Redirect("/");
            Book = new Book(id);
        }

        public void OnPost()
        {
            int id = Convert.ToInt32(Request.Form["BookID"]);
            string text = Request.Form["Title"];
            Book b = new Book(id);
            b.Title = text;
            b.Save();
            Book = b;
        }
    }
}