using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [Route("Model/DeleteBook")]
        public IActionResult DeleteBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Book book = new Book(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                book.IsDeleted = true;
                book.Quantity = 0;
                book.Save();
            }
            return new JsonResult("");
        }

        [Route("Model/UndeleteBook")]
        public IActionResult UndeleteBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Book book = new Book(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                book.IsDeleted = false;
                book.Quantity = 0;
                book.Save();
            }
            return new JsonResult("");
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

        [Route("Model/UndeleteAccount")]
        public IActionResult UndeleteAccount()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                user.IsDeleted = false;
                user.Save();
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

        [Route("Model/ClearReservations")]
        public IActionResult ClearReservations()
        {
            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser != null)
            {
                foreach (KeyValuePair<Book, int> item in currentUser.GetCart().AsEnumerable())
                {
                    item.Key.Quantity += item.Value;
                    item.Key.Save();
                }
                Request.HttpContext.Session.SetString("CheckingOut", "false");
            }
            return null;
        }

        [Route("Model/ValidateCart")]
        public IActionResult ValidateCart()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                JArray books = JArray.Parse(fields["Books"]);

                bool incorrectQuantity = false;
                foreach (JArray book in books.Children<JArray>())
                {
                    int BookID = (int)book[0];
                    int? Quantity = (int?)book[1];
                    if (!Quantity.HasValue) Quantity = 1;

                    Book b = new Book(BookID);
                    if (b.Quantity < Quantity)
                    {
                        book[1] = b.Quantity;
                        user.UpdateQuantityInCart(BookID, b.Quantity);
                        incorrectQuantity = true;
                    }
                }

                if (incorrectQuantity)
                    return new JsonResult(new object[] { false, books });
                else
                    return new JsonResult(new object[] { true });
            }
            return new JsonResult("");
        }

        [Route("Model/GetTotal")]
        public IActionResult GetTotal()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                JArray books = JArray.Parse(fields["Books"]);

                decimal BookTotal = 0;
                int Discount = 0;
                decimal OrderTotal = 0;

                foreach (JArray book in books.Children<JArray>())
                {
                    int BookID = (int) book[0];
                    int? Quantity = (int?)book[1];
                    if (!Quantity.HasValue) Quantity = 0;

                    user.UpdateQuantityInCart(BookID, Quantity.Value);
                    BookTotal += new Book(BookID).Price * Quantity.Value;
                }

                if (BookTotal > 75.00m) Discount = 10;
                OrderTotal = BookTotal - (BookTotal * (Discount / 100m));

                return new JsonResult(new object[] { BookTotal.ToString("0.00"),
                                                     Discount == 0 ? "0" : Discount + "%",
                                                    OrderTotal.ToString("0.00") });
            }
            return new JsonResult("");
        }

        [Route("Model/RemoveBook")]
        public IActionResult RemoveBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = new User(Convert.ToInt32(fields["UserID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == user.UserID || currentUser.IsAdmin)
            {
                user.RemoveBookFromCart(Convert.ToInt32(fields["BookID"]));
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
                if (a.Country == "US") a.State = fields["State"];
                a.ZIP = fields["ZIP"];
                c.Address = a;

                c.Number = fields["Number"];
                c.CVC = fields["CVC"];
                c.Name = fields["Name"];
                c.ExpMonth = fields["ExpMonth"];
                c.ExpYear = fields["ExpYear"];
                c.User = user;
                c.Save();
                return new JsonResult(new object[] { id, c.CardID });
            }
            return new JsonResult("");
        }

        [Route("Model/CreateOrder")]
        public IActionResult CreateOrder()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);

            Card c = new Card(Convert.ToInt32(fields["CardID"]));
            Address a = new Address(Convert.ToInt32(fields["AddressID"]));

            if (c.User.UserID != currentUser.UserID) return new JsonResult("");
            if (!c.DecryptNumber(fields["CVC"])) return new JsonResult("");
            c.CVC = fields["CVC"];

            //At this point send the Card data to the bank

            Order order = new Order();
            order.Card = c;
            order.Address = a;
            order.PercentDiscount = 0;

            decimal BookTotal = 0;
            int Discount = 0;

            foreach (KeyValuePair<Book, int> item in currentUser.GetCart().AsEnumerable())
            {
                order.Books.Add(item.Key, new { Price = item.Key.Price, Quantity = item.Value, Cost = item.Key.Cost });
                BookTotal += item.Key.Price * item.Value;
            }

            if (BookTotal > 75.00m) Discount = 10;
            order.PercentDiscount = Discount;

            order.Save();
            currentUser.EmptyCart();

            var t = Task.Run(() => Receipt.SendReceipt(order));

            return new JsonResult(order.OrderID);
        }

        [Route("Model/DeleteRating")]
        public IActionResult DeleteRating()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Rating r = new Rating(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.UserID == r.UserID || currentUser.IsAdmin)
            {
                r.Delete();
            }

            return new JsonResult("");
        }

        [Route("Model/SaveRating")]
        public IActionResult SaveRating()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = Authentication.GetCurrentUser(HttpContext);

            if (user != null)
            {
                int id = Convert.ToInt32(fields["ID"]);

                Rating r = id != -1 ? new Rating(id) : new Rating();

                if(id == -1)
                {
                    r.UserID = user.UserID;
                    r.BookID = Convert.ToInt32(fields["BookID"]);
                }

                r.RatingValue = Convert.ToInt32(fields["Rating"]);
                r.Comment = fields["Comment"];
                r.Save();

                return new JsonResult(JsonConvert.SerializeObject(r));
            }
            return new JsonResult(JsonConvert.SerializeObject(null));
        }

        [Route("Model/FulfillOrder")]
        public IActionResult FulfillOrder()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Order o = new Order(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                o.FulfilledDate = DateTime.Now;
                o.Save();
            }

            return new JsonResult("");
        }

        [Route("Model/RefundOrder")]
        public IActionResult RefundOrder()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Order o = new Order(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Card c = o.Card;
                String cardNumber = fields["CardNumber"];
                if (cardNumber.Length == 16 && c.LastFourDigits == cardNumber.Substring(12))
                {
                    c.Number = cardNumber;

                    //Send Card info with total to Bank;

                    o.RefundDate = DateTime.Now;
                    o.Save();

                    var t = Task.Run(() => Receipt.SendRefundReceipt(o));

                    return new JsonResult(true);
                }
                else
                {
                    return new JsonResult(false);
                }
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

        [Route("Model/AddToCart")]
        public IActionResult AddToCart()
        {
            User u = Authentication.GetCurrentUser(HttpContext);

            if(u == null)
            {
                return new JsonResult(JsonConvert.SerializeObject(0));
            }
            else
            {
                Dictionary<string, object> fields = AJAX.GetObjectFields(Request);
                int BookID = Convert.ToInt32(fields["BookID"]);
                if (u.CartContains(BookID))
                {
                    return new JsonResult(JsonConvert.SerializeObject(2));
                }
                else
                {
                    u.AddBookToCart(BookID);
                    return new JsonResult(JsonConvert.SerializeObject(1));
                }
            }
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

            if (search.StartsWith("$"))
                search = search.Replace("$", "");
            else
                search = "%" + search.Trim().Replace(" ", "%") + "%";

            DataTable bookresults = SQL.Execute(searchProcedure, new Param("@Search", search));
            List<Book> books = Book.GetBooks(bookresults);

            //This needs to get all books that have at least one cat in the cat list
            if(categories.Count() > 0)
                books = books.Where(b => b.Categories.Any(c => categories.Contains(c.CategoryID))).ToList();

            switch (sorttype)
            {
                case 2:
                    books.Sort((b1, b2) => b1.Categories.Where(c => categories.Contains(c.CategoryID)).Count().CompareTo(
                        b2.Categories.Where(c => categories.Contains(c.CategoryID)).Count()));
                    break;
                case 1:
                    books.Sort((b1, b2) => b2.Rating.CompareTo(b1.Rating));
                    break;
                case 3:
                    books.Sort((b1, b2) => b1.Price.CompareTo(b2.Price));
                    break;
                case 4:
                    books.Sort((b1, b2) => b2.Price.CompareTo(b1.Price));
                    break;
                case 5:
                    books.Sort((b1, b2) => b2.PublicationDate.CompareTo(b1.PublicationDate));
                    break;
                case 6:
                    books.Sort((b1, b2) => b1.PublicationDate.CompareTo(b2.PublicationDate));
                    break;
            }

            count = books.Count - index < count ? books.Count - index : count;
            books = books.GetRange(index, count);

            foreach (Book book in books)
            {
                book.LoadPublisher = false;
                book.Cost = 0;
            }

            return new JsonResult(JsonConvert.SerializeObject(new object[] { fields["CallID"], count, books }));
        }
    }
}