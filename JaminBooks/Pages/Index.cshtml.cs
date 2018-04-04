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

            Book book1 = new Book();
            DateTime da = DateTime.Now;

            book1.Title = "The Man of the Statue";
            book1.AFirstName = "Phil";
            book1.ALastName = "Bardson";
            book1.PublicationDate = da;
            book1.PublisherName = "SelfPublishing";
            book1.ISBN10 = "9876543210";
            book1.ISBN13 = "9876543210123";
            book1.Description = "Son of Bard Bardson, the Bard";
            book1.CategoryName = "Fantasy";
            book1.CopyrightDate = da;
            book1.Price = 10.99M;
            book1.Cost = 6.80M;
            book1.Quantity = 10;
            book1.PublisherName = "SelfPublishing";
            book1.AddressID = 2;
            book1.PhoneID = 12;
            book1.ContactFirstName = "Philson";
            book1.ContactLastName = "Bard";



            book1.Save();

        }
    }
}
