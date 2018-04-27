using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Displays the information of an order that has been placed.
    /// </summary>
    public class OrderModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The order to display.
        /// </summary>
        public Order Order;

        /// <summary>
        /// A list of ids of the order used to reship the current order.
        /// </summary>
        public List<int> Children;

        /// <summary>
        /// The total cost of the books.
        /// </summary>
        public string BookTotal;

        /// <summary>
        /// The discount on the order.
        /// </summary>
        public string PercentDiscount;

        /// <summary>
        /// The total value of the order.
        /// </summary>
        public string OrderTotal;

        /// <summary>
        /// Whether or not to display a thank you message.
        /// </summary>
        public bool DisplayThanks = false;

        /// <summary>
        /// Load the page on a post request. This is used when an order is created for the first time.
        /// </summary>
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

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="id">The id number of the order to display</param>
        public void OnGet(int id)
        {
            Order = new Order(id);

            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || (Order.Card.User.UserID != CurrentUser.UserID && !CurrentUser.IsAdmin))
            {
                Response.Redirect("Index");
            }
            else
            {
                RenderPage(Order);
            }
        }

        /// <summary>
        /// Render the fields of the given order on the page.
        /// </summary>
        /// <param name="order">The order to render</param>
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