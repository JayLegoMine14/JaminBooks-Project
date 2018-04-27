using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for searching publishers.
    /// </summary>
    public class PublishersModel : PageModel
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