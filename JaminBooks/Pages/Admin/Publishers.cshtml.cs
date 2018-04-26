using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Pages.Admin
{
    public class PublishersModel : PageModel
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

            DisplayColumns.Add("Publisher ID");
            DisplayColumns.Add("Publisher Name");
            DisplayColumns.Add("Contact Name");
            DisplayColumns.Add("Phone Number");
            DisplayColumns.Add("City");
            DisplayColumns.Add("Deleted");

            SearchColumns.Add("PublisherID", "PublisherID");
            SearchColumns.Add("PublisherName", "Publisher Name");
            SearchColumns.Add("FirstName", "Contact First Name");
            SearchColumns.Add("LastName", "Contact Last Name");
            SearchColumns.Add("PhoneNumber", "Phone Number");
            SearchColumns.Add("Books", "Books");
            SearchColumns.Add("Sales", "Sales");
            SearchColumns.Add("City", "City");
            SearchColumns.Add("State", "State");
            SearchColumns.Add("Country", "Country");
            SearchColumns.Add("ZIP", "ZIP");
            SearchColumns.Add("IsDeleted", "Deleted");
        }
    }
}