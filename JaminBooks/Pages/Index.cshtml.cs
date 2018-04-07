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

            Author a = new Author();

            Book book1 = new Book();
            Book book2 = new Book();
            byte[] result = new byte[100];
           
            DateTime da = DateTime.Now;

            book1.Title = "The book about nothing";
            book1.PublicationDate = da;
            book1.ISBN10 = "1234567890";
            book1.ISBN13 = "1234567890123";
            book1.Description = "The most interesting book you will ever read";
            book1.CategoryName = "Reference";
            book1.CopyrightDate = da;
            book1.Price = 0.01M;
            book1.Cost = 0.00M;
            book1.Quantity = 1000;
            book1.BookImage = result;

            book1.AFirstName = "Nemo";
            book1.ALastName = "Nihil";

            book1.PublisherName = "Nusquam Publishing";
            book1.ContactFirstName = "Nadia";
            book1.ContactLastName = "Outis";

            book1.Line1 = "0000 Nicht Road";
            book1.Line2 = "Apt #0";
            book1.City = "Nowhere";
            book1.State = "Oklahoma";
            book1.Country = "Turkey";
            book1.ZIP = "12345";

            book1.Number = "1(800)948-8488";
            book1.PhoneCategory = "Work";


            book1.Save();


        }
    }
}
