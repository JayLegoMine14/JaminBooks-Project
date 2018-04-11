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
    public class CheckoutModel : PageModel
    {
        public User CurrentUser;
        public string BookTotal;
        public string PercentDiscount;
        public string OrderTotal;

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || CurrentUser.GetCart().Count == 0)
            {
                Response.Redirect("Index");
            }
            else
            {
                decimal BookTotal = 0;
                int Discount = 0;
                decimal OrderTotal = 0;

                foreach (KeyValuePair<Book, int> item in CurrentUser.GetCart().AsEnumerable())
                    BookTotal += item.Key.Price * item.Value;

                if (BookTotal > 75.00m) Discount = 10;
                OrderTotal = BookTotal - (BookTotal * (Discount / 100m));

                this.BookTotal = "$" + BookTotal.ToString("0.00");
                this.PercentDiscount = Discount == 0 ? "" : Discount + "%";
                this.OrderTotal = "$" + OrderTotal.ToString("0.00");
            }
        }
    }
}