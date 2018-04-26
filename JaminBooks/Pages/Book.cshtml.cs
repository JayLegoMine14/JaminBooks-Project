using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    public class BookModel : PageModel
    {
        public Book Book;
        public User CurrentUser;
        public Promotion Promo;

        public void OnGet(int id)
        {
            Promotion.DeleteExpiredPromotions(id);
            Book = new Book(id);
            Promo = Promotions.GetPromotion(Book);
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
        }
    }
}