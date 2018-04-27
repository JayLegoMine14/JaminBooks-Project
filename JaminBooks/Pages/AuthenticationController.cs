using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Handles AJAX requests that deal with user authentication.
    /// </summary>
    public class AuthenticationController : Controller
    {
        /// <summary>
        /// Authenticates and logs in the given user.
        /// </summary>
        /// <returns>A boolean array with whether or not the login was successful at index 0 and whether or not
        /// the user is an administrator at index 1</returns>
        [Route("Security/Authenticate")]
        public IActionResult Authenticate()
        {
            return new JsonResult(Authentication.SetCurrentUser(Request));
        }

        /// <summary>
        /// Determines whether the given user already exists.
        /// </summary>
        /// <returns>A boolean indicating whether or not the user exists</returns>
        [Route("Security/Exists")]
        public IActionResult Exists()
        {
            return new JsonResult(Authentication.EmailExists(Request));
        }

        /// <summary>
        /// Determines whether the given user already exists, excepting the user who is logged in.
        /// </summary>
        /// <returns>A boolean indicating whether or not the user exists</returns>
        [Route("Security/ExistsWithException")]
        public IActionResult ExistsWithException()
        {
            return new JsonResult(Authentication.EmailExistsWithException(Request));
        }

        /// <summary>
        /// Creates a new user using the given data and logs them in.
        /// </summary>
        /// <returns>The new user's id number</returns>
        [Route("Security/Create")]
        public IActionResult Create()
        {
            return new JsonResult(Authentication.CreateUser(Request));
        }

        /// <summary>
        /// Creates a new user without requiring email confirmation or logging in.
        /// </summary>
        /// <returns>The new user's id number</returns>
        [Route("Security/AdminCreate")]
        public IActionResult AdminCreate()
        {
            return new JsonResult(Authentication.CreateUser(Request, true, false, false));
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        [Route("Security/Logout")]
        public void Logout()
        {
            var checkingOut = HttpContext.Session.GetString("CheckingOut");
            if (checkingOut != null && Convert.ToBoolean(checkingOut))
            {
                Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
                foreach (KeyValuePair<Book, int> item in currentUser.GetCart().AsEnumerable())
                {
                    item.Key.Quantity += item.Value;
                    item.Key.Save();
                }
            }

            Authentication.LogoutCurrentUser(HttpContext);
            Response.Redirect("/Index");
        }

        /// <summary>
        /// Confirms the email address of the user with the given confirmation code.
        /// </summary>
        /// <param name="id">The confirmation code</param>
        [Route("Security/Confirm")]
        public void Confirm(int id)
        {
            string code = HttpContext.Request.Query["c"].ToString();
            try
            {
                User u = new User(id);
                if (u.ConfirmationCode == code)
                {
                    u.IsConfirmed = true;
                    u.Save();
                    Response.Redirect("/Confirmed");
                }
                else Response.Redirect("/Error");
            }
            catch
            {
                Response.Redirect("/Error");
            }
        }
    }
}