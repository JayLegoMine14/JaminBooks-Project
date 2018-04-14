using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages.Admin
{
    public class OrdersModel : PageModel
    {
        public User CurrentUser;
        public List<string> DisplayColumns = new List<string>();
        public Dictionary<string, string> SearchColumns = new Dictionary<string, string>();

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
            SearchColumns.Add("PercentDiscount", "Dicount");
            SearchColumns.Add("IsFulfilled", "Is Fulfilled");
            SearchColumns.Add("IsRefunded", "Is Refunded");
        }
    }
}