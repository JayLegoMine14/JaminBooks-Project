using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays an interface for searching books.
    /// </summary>
    public class BooksModel : PageModel
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

            DisplayColumns.Add("Book ID");
            DisplayColumns.Add("Image");
            DisplayColumns.Add("Title");
            DisplayColumns.Add("Authors");
            DisplayColumns.Add("ISBN 13");
            DisplayColumns.Add("Price");
            DisplayColumns.Add("Cost");
            DisplayColumns.Add("Stock");
            DisplayColumns.Add("Deleted");

            SearchColumns.Add("BookID", "Book ID");
            SearchColumns.Add("Title", "Title");
            SearchColumns.Add("ISBN13", "ISBN 13");
            SearchColumns.Add("ISBN10", "ISBN 10");
            SearchColumns.Add("Description", "Description");
            SearchColumns.Add("Price", "Price");
            SearchColumns.Add("Cost", "Cost");
            SearchColumns.Add("Quantity", "Quantity");
            SearchColumns.Add("Rating", "Rating");
            SearchColumns.Add("Reviews", "Reviews");
            SearchColumns.Add("Sales", "Sales");
            SearchColumns.Add("Author", "Author");
            SearchColumns.Add("Category", "Category");
            SearchColumns.Add("CopyrightDate", "Copyright Date");
            SearchColumns.Add("PublisherName", "Publisher");
            SearchColumns.Add("PublicationDate", "PublicationDate");
            SearchColumns.Add("PublisherContact", "Publisher Contact");
            SearchColumns.Add("PercentDiscount", "Percent Discount");
            SearchColumns.Add("IsOnSale", "On Sale");
            SearchColumns.Add("IsDeleted", "Is Deleted");
        }
    }
}