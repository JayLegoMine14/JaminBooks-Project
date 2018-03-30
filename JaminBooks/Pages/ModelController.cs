using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace JaminBooks.Pages
{
    public class ModelController : Controller
    {
        [Route("Model/GetPhoneCategories")]
        public IActionResult Create()
        {
            return new JsonResult(JsonConvert.SerializeObject(Phone.GetPhoneCategories()));
        }

        [Route("Model/SaveUserIcon")]
        public IActionResult SaveIcon()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            byte[] blob = Convert.FromBase64String(fields["Icon"]);

            User u = Authentication.GetCurrentUser(HttpContext);
            u.Icon = blob;
            u.Save();
            return new JsonResult("");
        }
    }
}