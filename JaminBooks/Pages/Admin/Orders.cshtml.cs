using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for searching orders.
    /// </summary>
    public class OrdersModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The columns to display in the results table and the order by options.
        /// </summary>
        public List<string> DisplayColumns = new List<string>();

        /// <summary>
        /// The columns to display in the advanced search options.
        /// </summary>
        public Dictionary<string, string> SearchColumns = new Dictionary<string, string>();

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            DisplayColumns.Add("Order ID");
            DisplayColumns.Add("Order Date");
            DisplayColumns.Add("Customer");
            DisplayColumns.Add("Items");
            DisplayColumns.Add("Total");

            SearchColumns.Add("OrderID", "Order ID");
            SearchColumns.Add("OrderDate", "Order Date");
            SearchColumns.Add("FirstName", "First Name");
            SearchColumns.Add("LastName", "Last Name");
            SearchColumns.Add("Items", "Item Count");
            SearchColumns.Add("Item", "Item ISBN");
            SearchColumns.Add("Total", "Total");
            SearchColumns.Add("PercentDiscount", "Discount");
            SearchColumns.Add("IsReshipped", "Reshipped");
            SearchColumns.Add("IsReshipping", "Is Reship Order");
            SearchColumns.Add("IsFulfilled", "Is Fulfilled");
            SearchColumns.Add("FulfilledDate", "Date Fulfilled");
            SearchColumns.Add("RefundDate", "Date Refunded");
        }
    }
}