using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Displays the books in a user's cart, and calculates the order total.
    /// </summary>
    public class CartModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The user whose data is currently being displayed.
        /// </summary>
        public User DisplayUser;

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        /// <param name="id">The id number of the user whose data to load. (optional)</param>
        public void OnGet(int? id)
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null)
            {
                Response.Redirect("Index");
            }
            else
            {
                if (CurrentUser.IsAdmin && id != null)
                    DisplayUser = new User(id.Value);
                else
                    DisplayUser = CurrentUser;
            }
        }
    }
}