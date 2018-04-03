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

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);

<<<<<<< HEAD
            Book b1 = new Book();
            b1.Delete(4);
=======
            //Book b1 = new Book();
            //b1.Delete(4);
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5

            //DateTime DT = DateTime.Now;
            //Book book1 = new Book();
            //book1.AuthorID = 1;
            //book1.Title = "Happy Little Trees";
            //book1.PublicationDate = DT;
            //book1.PublisherID = 2;
            //book1.ISBN10 = "1234567890";
            //book1.ISBN13 = "1234567890123";
            //book1.Description = "Our Little Secret";
            //book1.CategoryID = "1";
            //book1.CopyrightDate = DT;
            //book1.Price = 14.99M;
            //book1.Cost = 7.47M;


            //book1.Save();


        }
    }
}
