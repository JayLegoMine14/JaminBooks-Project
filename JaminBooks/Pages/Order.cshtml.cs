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
    public class OrderModel : PageModel
    {
        public User CurrentUser;
        public Order Order;
        public List<int> Children;

        public string BookTotal;
        public string PercentDiscount;
        public string OrderTotal;

        public bool DisplayThanks = false;

        public void OnPost()
        {
            var id = Convert.ToInt32(Request.Form["id"]);
            DisplayThanks = Convert.ToBoolean(Request.Form["thanks"]);
            Order = new Order(id);

            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || Order.Card.User.UserID != CurrentUser.UserID)
            {
                Response.Redirect("Index");
            }
            else
            {
                RenderPage(Order);
            }
        }

        public void OnGet(int id)
        {
            Order = new Order(id);

            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || Order.Card.User.UserID != CurrentUser.UserID || !CurrentUser.IsAdmin)
            {
                Response.Redirect("Index");
            }
            else
            {
                RenderPage(Order);
            }
        }

        public void RenderPage(Order order)
        {
            decimal BookTotal = 0;
            int Discount = order.PercentDiscount;
            decimal OrderTotal = 0;

            foreach (KeyValuePair<Book, dynamic> item in order.Books)
                BookTotal += item.Value.Quantity * item.Value.Price;

            OrderTotal = BookTotal - (BookTotal * (Discount / 100m));

            this.BookTotal = "$" + BookTotal.ToString("0.00");
            this.PercentDiscount = Discount == 0 ? "" : Discount + "%";
            this.OrderTotal = "$" + OrderTotal.ToString("0.00");
            Children = order.Children;
        }
    }
}