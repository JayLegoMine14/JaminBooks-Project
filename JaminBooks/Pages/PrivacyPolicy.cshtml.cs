using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    public class PrivacyPolicyModel : PageModel
    {
        public User CurrentUser;

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
        }
    }
}