using System;
using System.Collections.Generic;
using System.Data;
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

        [Route("Model/GetBookCategories")]
        public IActionResult GetBookCategories()
        {
            DataTable dt = SQL.Execute("uspGetCategories");
            Dictionary<int, string> cats = new Dictionary<int, string>();
            foreach (DataRow dr in dt.Rows)
                cats.Add((int)dr["CategoryID"], (string)dr["CategoryName"]);
            return new JsonResult(JsonConvert.SerializeObject(cats));
        }

        [Route("Model/DeleteAccount")]
        public IActionResult DeleteAccount()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                user.Delete();
            }
            return new JsonResult(currentUser.UserID == user.UserID);
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

        [Route("Model/DeleteCard")]
        public IActionResult DeleteCard()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Card c = new Card(Convert.ToInt32(fields["ID"]));


            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == c.User.UserID || currentUser.IsAdmin)
            {
                c.Delete();
            }

            return new JsonResult("");
        }

        [Route("Model/DeleteAddress")]
        public IActionResult DeleteAddress()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Address a = new Address(Convert.ToInt32(fields["ID"]));


            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == a.GetUserID() || currentUser.IsAdmin)
            {
                a.Delete();
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

        [Route("Model/SaveCard")]
        public IActionResult SaveCard()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["ID"]);
                Card c = id != -1 ? new Card(id) : new Card();
                Address a = c.Address != null ? c.Address : new Address();

                a.Line1 = fields["Line1"];
                if (fields["Line2"] != "") a.Line2 = fields["Line2"];
                a.City = fields["City"];
                a.Country = fields["Country"];
                if(a.Country == "US") a.State = fields["State"];
                a.ZIP = fields["ZIP"];
                c.Address = a;

                c.Number = fields["Number"];
                c.CCV = fields["CCV"];
                c.Name = fields["Name"];
                c.ExpMonth = fields["ExpMonth"];
                c.ExpYear = fields["ExpYear"];
                c.User = user;
                c.Save();
                return new JsonResult(new object[] { id, c.CardID });
            }
            return new JsonResult("");
        }

        [Route("Model/SaveAddress")]
        public IActionResult SaveAddress()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["ID"]);
                Address a = id != -1 ? new Address(id) : new Address();

                a.Line1 = fields["Line1"];
                if (fields["Line2"] != "") a.Line2 = fields["Line2"];
                a.City = fields["City"];
                a.Country = fields["Country"];
                if (a.Country == "US") a.State = fields["State"];
                a.ZIP = fields["ZIP"];
                a.Save();
                user.AddAddress(a);

                return new JsonResult(new object[] { id, a.AddressID });
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
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                byte[] blob = Convert.FromBase64String(fields["Icon"]);

                user.Icon = blob;
                user.Save();
            }
            return new JsonResult("");
        }
    }
}