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
        public Author Author;
        public Publisher Publisher;
        public List<Publisher> PubList;
        public List<Author> Authors;
        public List<Category> Categories;

        public DateTime date1 = new DateTime(2000, 1, 1);

        public void OnGet(int? id)
        {
            User user = Authentication.GetCurrentUser(HttpContext);
            if (user == null || !user.IsAdmin) Response.Redirect("/");
            Book = (id == null ? null : new Book(id.Value));
        }

        public void OnPost()
        {
            int id = Convert.ToInt32(Request.Form["BookID"]);
            string title = Request.Form["Title"];
            DateTime PublicationDate = Convert.ToDateTime(Request.Form["PublicationDate"]);
            int PublisherID = Convert.ToInt32(Request.Form["PublisherID"]);
            string isbn10 = Request.Form["ISBN10"];
            string isbn13 = Request.Form["ISBN13"];
            string desc = Request.Form["Description"];
            DateTime CopyrightDate = Convert.ToDateTime(Request.Form["CopyrightDate"]);
            decimal price = Convert.ToDecimal(Request.Form["Price"]);
            decimal cost = Convert.ToDecimal(Request.Form["Cost"]);
            int quantity = Convert.ToInt32(Request.Form["Quantity"]);
            

            Book b = new Book(id);
            b.Title = title;
            b.Save();
            Book = b;
        }
    }
}