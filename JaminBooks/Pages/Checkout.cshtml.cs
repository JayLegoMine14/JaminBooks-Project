using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
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

        public void OnGet(string code)
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || CurrentUser.GetCart().Count == 0 || !CurrentUser.IsConfirmed)
            {
                Response.Redirect("Index");
            }
            else
            {
                decimal BookTotal = 0;
                int Discount = 0;
                decimal OrderTotal = 0;

                foreach (KeyValuePair<Book, int> item in CurrentUser.GetCart().AsEnumerable())
                {
                    item.Key.Quantity -= item.Value;
                    item.Key.Save();
                    BookTotal += item.Key.Price * item.Value;
                }

                if (!String.IsNullOrEmpty(code))
                    Discount = Promotions.GetDiscount(code);

                var totalDiscount = Promotions.GetDiscount(BookTotal);
                if (totalDiscount > Discount) Discount = totalDiscount;

                OrderTotal = BookTotal - (BookTotal * (Discount / 100m));

                this.BookTotal = "$" + BookTotal.ToString("0.00");
                this.PercentDiscount = Discount == 0 ? "" : Discount + "%";
                this.OrderTotal = "$" + OrderTotal.ToString("0.00");

                Request.HttpContext.Session.SetString("CheckingOut", "true");
            }
        }
    }
}