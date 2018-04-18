using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages.Admin
{
    public class AccountsModel : PageModel
    {

        public User CurrentUser;
        public List<string> DisplayColumns = new List<string>();
        public Dictionary<string, string> SearchColumns = new Dictionary<string, string>();

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if(CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            DisplayColumns.Add("User ID");
            DisplayColumns.Add("Icon");
            DisplayColumns.Add("Email");
            DisplayColumns.Add("Name");
            DisplayColumns.Add("CreationDate");
            DisplayColumns.Add("Enabled");

            SearchColumns.Add("UserID", "User ID");
            SearchColumns.Add("Email", "Email");
            SearchColumns.Add("CreationDate", "Creation Date");
            SearchColumns.Add("FirstName", "First Name");
            SearchColumns.Add("LastName", "Last Name");
            SearchColumns.Add("Orders", "Order Count");
            SearchColumns.Add("Book", "Book ISBN");
            SearchColumns.Add("Card", "Card Number");
            SearchColumns.Add("Phone", "Phone Number");
            SearchColumns.Add("City", "City");
            SearchColumns.Add("State", "State");
            SearchColumns.Add("Country", "Country");
            SearchColumns.Add("ZIP", "ZIP");
            SearchColumns.Add("IsEnabled", "Is Enabled");
            SearchColumns.Add("IsConfirmed", "Is Confirmed");
            SearchColumns.Add("IsAdmin", "Is Admin");
        }
    }
}