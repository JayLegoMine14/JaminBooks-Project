using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public IActionResult GetPhoneCategories()
        {
            return new JsonResult(JsonConvert.SerializeObject(Phone.GetPhoneCategories()));
        }

        [Route("Model/DeletePhone")]
        public IActionResult DeletePhone()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Phone p = new Phone(Convert.ToInt32(fields["ID"]));


            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == p.GetUserID() || currentUser.IsAdmin)
            {
                p.Delete();
            }

            return new JsonResult("");
        }

        [Route("Model/SavePhone")]
        public IActionResult SavePhone()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["ID"]);

                Phone p = id != -1 ? new Phone(id) : new Phone();

                p.Number = fields["Number"];
                p.Category = fields["Category"];
                p.Save();

                if (id == -1) user.AddPhone(p);

                return new JsonResult(p.PhoneID);
            }
            return new JsonResult("");
        }

        [Route("Model/ChangePassword")]
        public IActionResult ChangePassword()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                user.Password = Authentication.Hash(fields["Password"]);
                user.Save();
            }
            return new JsonResult("");
        }

        [Route("Model/UpdateUser")]
        public IActionResult UpdateUser()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                user.FirstName = fields["FirstName"];
                user.LastName = fields["LastName"];
                user.Email = fields["Email"];

                if (user.FirstName != "" &&
                    user.LastName != "" &&
                    user.FirstName.Length <= 50 &&
                    user.LastName.Length <= 50 &&
                    user.Email.Length <= 100 &&
                    new Regex("^(([^<>()[\\]\\.,;:\\s@\"]+(\\.[^<>()[\\]\\.,;:\\s@\"]+)*)|(\".+\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                    .IsMatch(user.Email))
                {
                    if (!user.IsConfirmed)
                    {
                        Authentication.SendConfirmationEmail(Request, user);
                        user.ConfirmationCode = Authentication.GenerateConfirmationCode();
                    }

                    user.Save();
                }
            }
            return new JsonResult("");
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