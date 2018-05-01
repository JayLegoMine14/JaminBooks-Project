using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Allows the user to select a means of payment and a shipping address and place an order.
    /// </summary>
    public class CheckoutModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The total price of the books.
        /// </summary>
        public string BookTotal;

        /// <summary>
        /// The discount on the order.
        /// </summary>
        public string PercentDiscount;

        /// <summary>
        /// The discount code applied.
        /// </summary>
        public string Code;

        /// <summary>
        /// The order total.
        /// </summary>
        public string OrderTotal;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="code">The discount code to be applied to the order</param>
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

                Code = code;
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