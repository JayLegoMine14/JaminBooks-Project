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
using Newtonsoft.Json.Linq;
using static JaminBooks.Model.SQL;

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

        [Route("Model/GetRatings")]
        public IActionResult GetRatings()
        {
            Dictionary<string, object> fields = AJAX.GetObjectFields(Request);
            int BookID = Convert.ToInt32(fields["BookID"]);
            return new JsonResult(JsonConvert.SerializeObject(Rating.GetRatings(BookID)));
        }

        [Route("Model/LoadBooks")]
        public IActionResult LoadBooks()
        {
            Dictionary<string, object> fields = AJAX.GetObjectFields(Request);

            int index = Convert.ToInt32(fields["Index"]);
            int count = Convert.ToInt32(fields["Count"]);

            string search = fields["Search"].ToString();
            int searchtype = Convert.ToInt32(fields["SearchType"]);
            int sorttype = Convert.ToInt32(fields["SortType"]);
            int[] categories = ((JArray)fields["Cats"]).Select(j => (int)j).ToArray();

            String searchProcedure = "uspSearchBookByAll";
            switch (searchtype) {
                case 1:
                    searchProcedure = "uspSearchBookByAll";
                    break;
                case 2:
                    searchProcedure = "uspSearchBookByTitle";
                    break;
                case 3:
                    searchProcedure = "uspSearchBookByAuthor";
                    break;
                case 4:
                    searchProcedure = "uspSearchBookByPublisher";
                    break;
                case 5:
                    searchProcedure = "uspSearchBookByISBN";
                    break;
            }

            search = "%" + search.Trim().Replace(" ", "%") + "%";
            DataTable bookresults = SQL.Execute(searchProcedure, new Param("@Search", search));
            List<Book> books = Book.GetBooks(bookresults);

            //This needs to get all books that have at least one cat in the cat list
            if(categories.Count() > 0)
                books = books.Where(b => b.Categories.Any(c => categories.Contains(c.CategoryID))).ToList();

            switch (sorttype)
            {
                case 1:
                    books.Sort((b1, b2) => 1.CompareTo(1));
                    break;
                case 2:
                    books.Sort((b1, b2) => b1.Price.CompareTo(b2.Price));
                    break;
                case 3:
                    books.Sort((b1, b2) => b2.Price.CompareTo(b1.Price));
                    break;
                case 4:
                    books.Sort((b1, b2) => b1.PublicationDate.CompareTo(b2.PublicationDate));
                    break;
                case 5:
                    books.Sort((b1, b2) => b2.PublicationDate.CompareTo(b1.PublicationDate));
                    break;
            }

            count = books.Count - index < count ? books.Count - index : count;
            books = books.GetRange(index, count);

            return new JsonResult(JsonConvert.SerializeObject(new object[] { count, books }));
        }
    }
}