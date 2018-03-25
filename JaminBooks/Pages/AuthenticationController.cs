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
    }
}