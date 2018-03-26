using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;

namespace JaminBooks.Pages
{
    public class AuthenticationController : Controller
    {
        [Route("Security/Authenticate")]
        public IActionResult Authenticate()
        {
            return new JsonResult(Authentication.SetCurrentUser(Request));
        }

        [Route("Security/Exists")]
        public IActionResult Exists()
        {
            return new JsonResult(Authentication.EmailExists(Request));
        }

        [Route("Security/Create")]
        public IActionResult Create()
        {
            return new JsonResult(Authentication.CreateUser(Request));
        }

        [Route("Security/Logout")]
        public void Logout()
        {
            Authentication.LogoutCurrentUser(HttpContext);
            Response.Redirect("/Index");
        }
    }
}