using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Pages
{
    /// <summary>
    /// Handles AJAX calls that modify model objects.
    /// </summary>
    public class ModelController : Controller
    {
        #region Phone

        /// <summary>
        /// Saves changes to a phone.
        /// </summary>
        /// <returns>The phone's id number</returns>
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

        /// <summary>
        /// Deletes a phone from the database.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Get a list of all the phone categories.
        /// </summary>
        /// <returns>A list of phone categories and their ids</returns>
        [Route("Model/GetPhoneCategories")]
        public IActionResult GetPhoneCategories()
        {
            return new JsonResult(JsonConvert.SerializeObject(Phone.GetPhoneCategories()));
        }

        #endregion Phone

        #region Books

        /// <summary>
        /// Saves changes to a book or creates a new book
        /// </summary>
        /// <returns>The books id number</returns>
        [Route("Model/SaveBook")]
        public IActionResult SaveBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Book book = new Book();

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["BookID"]);

                Book b = id != -1 ? new Book(id) : new Book();
                Author auth = new Author();
                Category cat = new Category();

                byte[] blob = Convert.FromBase64String(fields["Image"]);

                if (!String.IsNullOrWhiteSpace(fields["Image"]))
                    b.BookImage = blob;
                b.Title = fields["Title"];
                b.PublicationDate = Convert.ToDateTime(fields["PublicationDate"]);
                b.CopyrightDate = Convert.ToDateTime(fields["CopyrightDate"]);

                b.PublisherID = Convert.ToInt32(fields["Publisher"]);
                b.ISBN10 = fields["ISBN10"];
                b.ISBN13 = fields["ISBN13"];
                b.Price = Convert.ToDecimal(fields["Price"]);
                b.Cost = Convert.ToDecimal(fields["Cost"]);
                b.Quantity = Convert.ToInt32(fields["Quantity"]);
                b.Description = fields["Description"];

                b.Save();

                foreach (Author a in b.Authors) a.DeleteAuthorFromBook(id);
                foreach (Category c in b.Categories) c.DeleteCategoryFromBook(id);

                int[] categories = JsonConvert.DeserializeObject<int[]>(fields["BookCat"]);
                foreach (int c in categories)
                    b.AddCategory(new Category(c));

                int[] authors = JsonConvert.DeserializeObject<int[]>(fields["Authorlist"]);
                foreach (int a in authors)
                    b.AddAuthor(new Author(a));

                //to ensure the cleanliness of the table, all unattended Authors and categories will be removed from the premises
                auth.DumpAuthors();
                cat.DumpCategories();

                return new JsonResult(b.BookID);
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Sets the given book as deleted.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/DeleteBook")]
        public IActionResult DeleteBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Book book = new Book(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                book.IsDeleted = true;
                book.Save();
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Set the given book as not deleted.
        /// </summary>
        /// <returns>0 to signify completion</returns>
        [Route("Model/UndeleteBook")]
        public IActionResult UndeleteBook()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Book book = new Book(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                if (!book.Publisher.IsDeleted)
                {
                    book.IsDeleted = false;
                    book.Save();
                }
                else
                {
                    return new JsonResult(0);
                }
            }
            return new JsonResult(1);
        }

        /// <summary>
        /// Get a list of all book categories.
        /// </summary>
        /// <returns>A list of all book categories and their ids</returns>
        [Route("Model/GetBookCategories")]
        public IActionResult GetBookCategories()
        {
            return new JsonResult(JsonConvert.SerializeObject(Category.GetCategories()));
        }

        /// <summary>
        /// Load a list of books that match the current search.
        /// </summary>
        /// <returns>An object array containing the AJAX call id at index 0, the number of books returned at index 1,
        /// the list of book at index 2, the current users bookshelf at index 3, and the a list of titles and authors to display
        /// in the auto complete box at index 4</returns>
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
            switch (searchtype)
            {
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

            List<String> autofills = SQL.Execute("uspGetAutofill", new Param("@Search", search))
                .Select().Select(row => row["Match"].ToString()).ToList();

            DataTable bookresults = SQL.Execute(searchProcedure, new Param("@Search", search));
            List<Book> books = Book.GetBooks(bookresults);
            books = books.Where(b => !b.IsDeleted).ToList();

            //This needs to get all books that have at least one cat in the cat list
            if (categories.Count() > 0)
                books = books.Where(b => b.Categories.Any(c => categories.Contains(c.CategoryID))).ToList();

            switch (sorttype)
            {
                case 2:
                    books.Sort((b1, b2) => b2.Categories.Where(c => categories.Contains(c.CategoryID)).Count().CompareTo(
                        b1.Categories.Where(c => categories.Contains(c.CategoryID)).Count()));
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

                case 7:
                    books.Sort((b1, b2) => b2.PercentDiscount.CompareTo(b1.PercentDiscount));
                    break;
            }

            count = books.Count - index < count ? books.Count - index : count;
            books = books.GetRange(index, count);

            foreach (Book book in books)
            {
                book.LoadPublisher = false;
                book.Cost = 0;
            }

            User u = Authentication.GetCurrentUser(HttpContext);
            List<int> bookshelf = u == null ? new List<int>() : u.GetBookShelf().Select(b => b.BookID).ToList();

            return new JsonResult(JsonConvert.SerializeObject(new object[] { fields["CallID"], count, books, bookshelf, autofills }));
        }

        /// <summary>
        /// Add the given book to the current user's bookshelf.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/AddBookToBookShelf")]
        public IActionResult AddBookToBookShelf()
        {
            User u = Authentication.GetCurrentUser(HttpContext);
            Dictionary<string, object> fields = AJAX.GetObjectFields(Request);
            u.AddBookToBookShelf(Convert.ToInt32(fields["ID"]));
            return new JsonResult("");
        }

        /// <summary>
        /// Remove the given book from the current user's bookshelf.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/RemoveBookFromBookShelf")]
        public IActionResult RemoveBookFromBookShelf()
        {
            User u = Authentication.GetCurrentUser(HttpContext);
            Dictionary<string, object> fields = AJAX.GetObjectFields(Request);
            u.RemoveBookFromBookShelf(Convert.ToInt32(fields["ID"]));
            return new JsonResult("");
        }

        #endregion Books

        #region Publishers

        /// <summary>
        /// Saves changes to the publisher or create a new publisher
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/SavePublisher")]
        public ActionResult SavePublisher()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["ID"]);
                Publisher p = id != -1 ? new Publisher(id) : new Publisher();

                p.PublisherName = fields["PublisherName"].ToString();
                p.ContactFirstName = fields["ContactFirstName"].ToString();
                p.ContactLastName = fields["ContactLastName"].ToString();

                Phone ph = p.PublisherID == -1 ? new Phone() : new Phone(p.Phone.PhoneID);
                ph.Number = fields["PhoneNumber"].ToString();
                ph.Category = fields["PhoneCategory"].ToString();
                ph.Save();

                p.Phone = ph;

                Address a = p.PublisherID == -1 ? new Address() : new Address(p.Address.AddressID);
                a.Line1 = fields["Line1"].ToString();
                a.Line2 = fields["Line2"].ToString();
                a.City = fields["City"].ToString();
                if (a.Country == "US") a.State = fields["State"];
                a.Country = fields["Country"].ToString();
                a.ZIP = fields["ZIP"].ToString();
                a.Save();

                p.Address = a;
                p.Save();
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Create a new publisher.
        /// </summary>
        /// <returns>The publisher's id</returns>
        [Route("Model/CreatePublisher")]
        public IActionResult CreatePublisher()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Publisher publisher = Convert.ToInt32(fields["PublisherID"]) == -1 ? new Publisher() : new Publisher(Convert.ToInt32(fields["PublisherID"]));
            Model.Phone phone = Convert.ToInt32(fields["PhoneID"]) == -1 ? new Phone() : new Phone(Convert.ToInt32(fields["PhoneID"]));
            Model.Address address = Convert.ToInt32(fields["AddressID"]) == -1 ? new Address() : new Address(Convert.ToInt32(fields["AddressID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["PublisherID"]);

                Publisher p = id != -1 ? new Publisher(id) : new Publisher();
                Phone ph = id != -1 ? new Phone(id) : new Phone();
                Address ad = id != -1 ? new Address(id) : new Address();

                p.PublisherName = fields["PublisherName"];
                p.ContactFirstName = fields["FirstName"];
                p.ContactLastName = fields["LastName"];

                ph.Save();
                ad.Save();

                p.Save();
            }
            return new JsonResult(publisher.PublisherID);
        }

        /// <summary>
        /// Deletes the given publisher.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/DeletePublisher")]
        public IActionResult DeletePublisher()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Publisher pub = new Publisher(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                pub.Delete();
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Undelete the given publisher.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/UndeletePublisher")]
        public IActionResult UndeletePublisher()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Publisher pub = new Publisher(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                pub.IsDeleted = false;
                pub.Save();
            }
            return new JsonResult("");
        }

        #endregion Publishers

        #region Users

        /// <summary>
        /// Change the given user's password.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Update the name and email address of the given user
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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
                        user.ConfirmationCode = Authentication.GenerateConfirmationCode();
                        Task.Run(() => Authentication.SendConfirmationEmail(Request, user));
                    }

                    if (currentUser.IsAdmin)
                        if (fields.ContainsKey("IsAdmin"))
                            user.IsAdmin = Convert.ToBoolean(fields["IsAdmin"]);

                    user.Save();
                }
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Set the given user's icon.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Disable the current user's account.
        /// </summary>
        /// <returns>Whether the current user or an administrator has disabled the account</returns>
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

        /// <summary>
        /// Undelete the given user.
        /// </summary>
        /// <returns>Whether the current user or an administrator has disabled the account</returns>
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

        #endregion Users

        #region Cards

        /// <summary>
        /// Save changes to a card or create a new card.
        /// </summary>
        /// <returns>The card's id number, the old card's id number, and the id number of the card's address</returns>
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
                Address a = c.Address ?? new Address();

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
                return new JsonResult(new object[] { id, c.CardID, c.Address.AddressID });
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Delete the given card.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        #endregion Cards

        #region Addresses

        /// <summary>
        /// Save changes to an address or create a new address.
        /// </summary>
        /// <returns>The address's id number and the old address's id number</returns>
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

        /// <summary>
        /// Sets an address as deleted.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        #endregion Addresses

        #region Cart

        /// <summary>
        /// Calculate the total value of the items in a user's cart.
        /// </summary>
        /// <returns>The AJAX call id, whether or not a good discount code was applied, the total for the books,
        /// the discount, and the total for the entire order</returns>
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
                    int BookID = (int)book[0];
                    int? Quantity = (int?)book[1];
                    if (!Quantity.HasValue) Quantity = 0;

                    user.UpdateQuantityInCart(BookID, Quantity.Value);
                    BookTotal += new Book(BookID).Price * Quantity.Value;
                }

                var codeWorks = false;
                string code = fields["Code"];
                if (!String.IsNullOrEmpty(code))
                {
                    Discount = Promotions.GetDiscount(code);
                    if (Discount > 0) codeWorks = true;
                }

                var totalDiscount = Promotions.GetDiscount(BookTotal);
                if (totalDiscount > Discount) Discount = totalDiscount;

                OrderTotal = BookTotal - (BookTotal * (Discount / 100m));

                return new JsonResult(new object[] { fields["CallID"],
                                                     codeWorks,
                                                     BookTotal.ToString("0.00"),
                                                     Discount == 0 ? "0" : Discount + "%",
                                                     OrderTotal.ToString("0.00") });
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Verifies that all of the items in the users cart are in stock.
        /// </summary>
        /// <returns>A boolean indicating whether or not the cart is valid and a list of books
        /// that are out of stock or have insufficient stock</returns>
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

        /// <summary>
        /// Adds the given book to the user's cart.
        /// </summary>
        /// <returns>0 is the user is not logged in, 1 if successful, and 2 if the book is already in the cart</returns>
        [Route("Model/AddToCart")]
        public IActionResult AddToCart()
        {
            User u = Authentication.GetCurrentUser(HttpContext);

            if (u == null)
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

        /// <summary>
        /// Removes a book from the given user's cart.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Set the books in a user's cart back into the inventory.
        /// </summary>
        /// <returns>An null to signify completion</returns>
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

        #endregion Cart

        #region Banners

        /// <summary>
        /// Creates a new banner.
        /// </summary>
        /// <returns>The banner's id</returns>
        [Route("Model/AddBanner")]
        public IActionResult SaveBanner()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                byte[] blob = Convert.FromBase64String(fields["Image"]);
                string URL = fields["URL"].ToString();
                Banner b = new Banner
                {
                    Image = blob,
                    URL = URL == "" ? null : URL,
                    Order = Convert.ToInt32(fields["Order"])
                };
                b.Save();
                return new JsonResult(b.BannerID);
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Changes the order of the banners
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/OrderBanners")]
        public IActionResult OrderBanners()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                int[] banners = JsonConvert.DeserializeObject<int[]>(fields["Order"]);

                int order = 0;
                foreach (int id in banners)
                {
                    Banner.SetOrder(id, order++);
                }
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Deletes the given banner.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/RemoveBanner")]
        public IActionResult RemoveBanner()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Banner.Delete(Convert.ToInt32(fields["ID"]));
            }

            return new JsonResult("");
        }

        #endregion Banners

        #region Ratings

        /// <summary>
        /// Save change to the given rating or create a new rating.
        /// </summary>
        /// <returns>The rating</returns>
        [Route("Model/SaveRating")]
        public IActionResult SaveRating()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User user = Authentication.GetCurrentUser(HttpContext);

            if (user != null)
            {
                int id = Convert.ToInt32(fields["ID"]);

                Rating r = id != -1 ? new Rating(id) : new Rating();

                if (id == -1)
                {
                    r.UserID = user.UserID;
                    r.BookID = Convert.ToInt32(fields["BookID"]);
                }
                if (!r.Hidden)
                {
                    r.RatingValue = Convert.ToInt32(fields["Rating"]);
                    r.Comment = ProfanityFilter.Filter(fields["Comment"]);
                }
                r.Save();

                return new JsonResult(JsonConvert.SerializeObject(r));
            }
            return new JsonResult(JsonConvert.SerializeObject(null));
        }

        /// <summary>
        /// Deletes the given rating.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Add a flag to the given rating.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/FlagRating")]
        public IActionResult FlagRating()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Rating r = new Rating(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser != null && !currentUser.IsAdmin && r.Comment != "" && !r.hasFlagged(currentUser.UserID))
            {
                r.AddFlag(currentUser.UserID);
            }

            return new JsonResult("");
        }

        /// <summary>
        /// Set the given rating as hidden.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/HideRating")]
        public IActionResult HideRating()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Rating r = new Rating(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                r.Hidden = true;
                r.DeleteFlags();
                r.Save();
            }

            return new JsonResult("");
        }

        /// <summary>
        /// Remove all flags from the given rating.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/ClearFlags")]
        public IActionResult ClearFlags()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Rating r = new Rating(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                r.DeleteFlags();
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Get all of the ratings on a given book.
        /// </summary>
        /// <returns>A list of ratings</returns>
        [Route("Model/GetRatings")]
        public IActionResult GetRatings()
        {
            Dictionary<string, object> fields = AJAX.GetObjectFields(Request);
            int BookID = Convert.ToInt32(fields["BookID"]);
            List<Rating> ratings = Rating.GetRatings(BookID);
            return new JsonResult(JsonConvert.SerializeObject(ratings.Where(r => !r.Hidden).ToList()));
        }

        #endregion Ratings

        #region Orders

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <returns>The order's id</returns>
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

            Order order = new Order
            {
                Card = c,
                Address = a,
                PercentDiscount = 0
            };

            decimal BookTotal = 0;
            int Discount = 0;

            foreach (KeyValuePair<Book, int> item in currentUser.GetCart().AsEnumerable())
            {
                order.Books.Add(item.Key, new { item.Key.Price, Quantity = item.Value, item.Key.Cost });
                BookTotal += item.Key.Price * item.Value;
            }

            if (!String.IsNullOrEmpty(fields["Code"]))
                Discount = Promotions.GetDiscount(fields["Code"]);

            var totalDiscount = Promotions.GetDiscount(BookTotal);
            if (totalDiscount > Discount) Discount = totalDiscount;

            order.PercentDiscount = Discount;

            order.Save();
            currentUser.EmptyCart();

            var t = Task.Run(() => Receipt.SendReceipt(order));

            return new JsonResult(order.OrderID);
        }

        /// <summary>
        /// Set the given order as fulfilled.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
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

        /// <summary>
        /// Reship the given order.
        /// </summary>
        /// <returns>0 if the order has already been returned twice, 1 followed by a list of titles if some titles are out
        /// of stock, and 2 if successful followed by the new order id.</returns>
        [Route("Model/ReshipOrder")]
        public IActionResult ReshipOrder()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Order o = new Order(Convert.ToInt32(fields["ID"]));

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                List<string> titles = new List<string>();
                foreach (KeyValuePair<Book, dynamic> book in o.Books)
                {
                    if (book.Key.Quantity < book.Value.Quantity)
                        titles.Add(book.Key.Title);
                }

                if (o.Children.Count == 2)
                {
                    return new JsonResult(new object[] { 0 });
                }
                else if (titles.Count > 0)
                {
                    return new JsonResult(new object[] { 1, titles });
                }
                else
                {
                    int oldID = o.OrderID;
                    o.ClearID();
                    o.FulfilledDate = DateTime.Now;
                    o.ParentOrderID = oldID;
                    o.PercentDiscount = 100;
                    o.Save();
                    return new JsonResult(new object[] { 2, o.OrderID });
                }
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Refund the given order.
        /// </summary>
        /// <returns>A boolean indicating whether or not the given card number is correct</returns>
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

        #endregion Orders

        #region Promotions

        /// <summary>
        /// Save changes to a promotion or create a new promotion.
        /// </summary>
        /// <returns>The promotions id number.</returns>
        [Route("Model/SavePromotion")]
        public IActionResult SavePromotion()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                int id = Convert.ToInt32(fields["ID"]);
                Promotion p = id != -1 ? new Promotion(id) : new Promotion();

                p.Total = fields["Total"] == null ? null : (decimal?)Convert.ToDecimal(fields["Total"]);
                p.Code = fields["Code"] ?? null;
                p.PercentDiscount = Convert.ToInt32(fields["Percent"]);
                p.StartDate = DateTime.Parse(fields["Startdate"]);
                p.EndDate = DateTime.Parse(fields["Enddate"]);
                p.Save();

                return new JsonResult(new object[] { p.PromotionID });
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Add a discount to the given book.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/AddDiscount")]
        public IActionResult AddDiscount()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Promotion p = new Promotion
                {
                    BookID = Convert.ToInt32(fields["ID"]),
                    PercentDiscount = Convert.ToInt32(fields["Percent"]),
                    StartDate = DateTime.Parse(fields["Startdate"]),
                    EndDate = DateTime.Parse(fields["Enddate"])
                };
                p.Save();
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Remove all discounts from the given book.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/RemoveDiscount")]
        public IActionResult RemoveDiscount()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Promotion.DeletePromotions(Convert.ToInt32(fields["ID"]));
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Deletes the given promotion.
        /// </summary>
        /// <returns>An empty JSON object to signify completion</returns>
        [Route("Model/DeletePromotion")]
        public IActionResult DeletePromotion()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Promotion p = new Promotion(Convert.ToInt32(fields["ID"]));
                p.Delete();
            }
            return new JsonResult("");
        }

        #endregion Promotions

        /// <summary>
        /// Create a new author.
        /// </summary>
        /// <returns>The author's id and name</returns>
        [Route("Model/CreateAuthor")]
        public IActionResult CreateAuthor()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Author author = new Author();

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Author exists = Author.GetAuthors().FirstOrDefault(a => a.FullName == fields["AFirstName"] + " " + fields["ALastName"]);
                if (exists != null)
                {
                    return new JsonResult(new object[] { exists.AuthorID, exists.FullName });
                }
                else
                {
                    Author a = new Author
                    {
                        FirstName = fields["AFirstName"],
                        LastName = fields["ALastName"]
                    };

                    a.Save();

                    return new JsonResult(new object[] { a.AuthorID, a.FullName });
                }
            }
            return new JsonResult("");
        }

        /// <summary>
        /// Create a new category.
        /// </summary>
        /// <returns>The categories id and name</returns>
        [Route("Model/CreateCategory")]
        public IActionResult CreateCategory()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            Model.Category category = new Category();

            Model.User currentUser = Authentication.GetCurrentUser(HttpContext);
            if (currentUser.IsAdmin)
            {
                Category exists = Category.GetCategories().FirstOrDefault(cat => cat.CategoryName == fields["CategoryName"]);
                if (exists != null)
                {
                    return new JsonResult(new object[] { exists.CategoryID, exists.CategoryName });
                }
                else
                {
                    Category c = new Category
                    {
                        CategoryName = fields["CategoryName"],
                        IsDeleted = false
                    };

                    c.Save();
                    return new JsonResult(new object[] { c.CategoryID, c.CategoryName });
                }
            }
            return new JsonResult("");
        }
    }
}