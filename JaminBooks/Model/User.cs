using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a user
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique id number that identifies the user. -1 represents an uncreated user.
        /// </summary>
        public int UserID { private set; get; } = -1;
        /// <summary>
        /// The creation date of the user.
        /// </summary>
        public DateTime CreationDate { private set; get; }
        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName;
        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName;
        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email;
        /// <summary>
        /// Whether or not the user is deleted.
        /// </summary>
        public bool IsDeleted = false;
        /// <summary>
        /// Whether or not the user is an administrator.
        /// </summary>
        public bool IsAdmin = false;
        /// <summary>
        /// Whether or not the user's email is confirmed.
        /// </summary>
        public bool IsConfirmed = false;
        /// <summary>
        /// The confirmation code of the given user.
        /// </summary>
        public string ConfirmationCode;
        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password;
        /// <summary>
        /// The icon of the user.
        /// </summary>
        public byte[] Icon { private get; set; }

        /// <summary>
        /// Whether or not the user has an icon
        /// </summary>
        public bool HasIcon
        {
            get
            {
                return Icon != null;
            }
        }

        /// <summary>
        /// The first and last name of the user joined by a comma, last name first.
        /// </summary>
        public string LastFirstName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        /// <summary>
        /// The first and last name of the user joined by a space.
        /// </summary>
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Cache this user's image if the image has not yet been cached. If the image has been cached, return the path to the image. 
        /// </summary>
        public string LoadImage
        {
            get
            {
                if (Icon == null) return "/images/user.png";
                var filename = Authentication.Hash(Convert.ToBase64String(Icon)) + ".png";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "temp");
                Directory.CreateDirectory(path);
                path = Path.Combine(path, filename);
                if (!File.Exists(path))
                {
                    using (MemoryStream ms = new MemoryStream(Icon))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }

                return "/images/temp/" + filename;
            }
        }

        /// <summary>
        /// A list of the orders made by the user.
        /// </summary>
        public List<Order> Orders
        {
            get
            {
                return Order.GetAllByUser(this.UserID);
            }
        }

        /// <summary>
        /// A list of the user's shipping addresses.
        /// </summary>
        public List<Address> Addresses
        {
            get
            {
                return Address.GetAddresses(this.UserID);
            }
        }

        /// <summary>
        /// A list of the user's sipping and billing addresses.
        /// </summary>
        public List<Address> AllAddresses
        {
            get
            {
                return Address.GetAddressesIncludingCards(this.UserID);
            }
        }

        /// <summary>
        /// A list of the user's cards.
        /// </summary>
        public List<Card> Cards
        {
            get
            {
                return Card.GetCards(this.UserID);
            }
        }

        /// <summary>
        /// A list of the user's phones.
        /// </summary>
        public List<Phone> Phones
        {
            get
            {
                return Phone.GetPhones(this.UserID);
            }
        }

        /// <summary>
        /// Initialize an empty user with default values.
        /// </summary>
        public User() { }

        /// <summary>
        /// Initialize a user and set the fields equal to the user in the database with the given id.
        /// </summary>
        /// <param name="UserID"></param>
        public User(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetUserByID", new Param("UserID", UserID));

            if (dt.Rows.Count > 0)
            {
                this.UserID = UserID;
                this.FirstName = (String)dt.Rows[0]["FirstName"];
                this.LastName = (String)dt.Rows[0]["LastName"];
                this.CreationDate = (DateTime)dt.Rows[0]["CreationDate"];
                this.Password = (String)dt.Rows[0]["Password"];
                this.Email = (String)dt.Rows[0]["Email"];
                this.IsDeleted = (Boolean)dt.Rows[0]["IsDeleted"];
                this.IsAdmin = (Boolean)dt.Rows[0]["IsAdmin"];
                this.IsConfirmed = (Boolean)dt.Rows[0]["IsConfirmed"];
                this.ConfirmationCode = dt.Rows[0]["ConfirmationCode"] == DBNull.Value ? null : (String)dt.Rows[0]["ConfirmationCode"];
                this.Icon = dt.Rows[0]["Icon"] == DBNull.Value ? null : (byte[])dt.Rows[0]["Icon"];
            }
            else
            {
                throw new Exception("Invalid User ID");
            }
        }

        /// <summary>
        /// Initialize a user and set its fields equal to the given parameters.
        /// </summary>
        /// <param name="UserID">The user's id</param>
        /// <param name="FirstName">The user's first name</param>
        /// <param name="LastName">The user's last name</param>
        /// <param name="CreationDate">The date the user was created</param>
        /// <param name="Password">The user's password</param>
        /// <param name="Email">The user's email address</param>
        /// <param name="IsDeleted">Whether or not the user is deleted</param>
        /// <param name="IsAdmin">Whether or not the user is an administrator</param>
        /// <param name="IsConfirmed">Whether or not the user's email is confirmed</param>
        /// <param name="ConfirmationCode">The user's email confirmation code</param>
        /// <param name="Icon">The user's icon</param>
        private User(int UserID, string FirstName, string LastName, DateTime CreationDate,
            string Password, string Email, bool IsDeleted, bool IsAdmin, bool IsConfirmed,
            string ConfirmationCode, byte[] Icon)
        {
            this.UserID = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.CreationDate = CreationDate;
            this.Password = Password;
            this.Email = Email;
            this.IsDeleted = IsDeleted;
            this.IsAdmin = IsAdmin;
            this.IsConfirmed = IsConfirmed;
            this.ConfirmationCode = ConfirmationCode;
            this.Icon = Icon;
        }

        /// <summary>
        /// Save the user to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveUser",
                new Param("UserID", UserID),
                new Param("FirstName", FirstName),
                new Param("LastName", LastName),
                new Param("Email", Email),
                new Param("Password", Password),
                new Param("IsDeleted", IsDeleted),
                new Param("IsAdmin", IsAdmin),
                new Param("IsConfirmed", IsConfirmed),
                new Param("ConfirmationCode", ConfirmationCode),
                new Param("Icon", Icon ?? SqlBinary.Null));

            if (dt.Rows.Count > 0)
                UserID = (int)dt.Rows[0]["UserID"];
        }

        /// <summary>
        /// Delete the user from the database.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteUser", new Param("UserID", UserID));
            UserID = -1;
        }

        /// <summary>
        /// Add the given address to the user.
        /// </summary>
        /// <param name="a">The address to add</param>
        public void AddAddress(Address a)
        {
            a.AddUser(this.UserID);
        }

        /// <summary>
        /// Add the given card to the user.
        /// </summary>
        /// <param name="c">The card to add</param>
        public void AddCard(Card c)
        {
            c.User = this;
        }

        /// <summary>
        /// Add the given phone to the user.
        /// </summary>
        /// <param name="p">The phone to add</param>
        public void AddPhone(Phone p)
        {
            p.AddUser(this.UserID);
        }

        /// <summary>
        /// Check whether or not the users cart contains the given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        /// <returns>whether or not the users cart contains the given book</returns>
        public bool CartContains(int BookID)
        {
            DataTable bookresults = SQL.Execute("uspGetCart", new Param("UserID", UserID));
            List<Book> books = Book.GetBooks(bookresults);
            return books.Any(b => b.BookID == BookID);
        }

        /// <summary>
        /// Get a user's cart.
        /// </summary>
        /// <returns>A dictionary containing books and their quantity</returns>
        public Dictionary<Book, int> GetCart()
        {
            DataTable bookresults = SQL.Execute("uspGetCart", new Param("UserID", UserID));
            List<Book> books = Book.GetBooks(bookresults);

            Dictionary<Book, int> cartItems = new Dictionary<Book, int>();
            int i = 0;
            foreach (Book book in books)
            {
                book.Publisher.Address = null;
                book.Publisher.ContactFirstName = "";
                book.Publisher.ContactLastName = "";
                book.Publisher.Phone = null;
                book.Cost = 0;
                cartItems.Add(book, Convert.ToInt32(bookresults.Rows[i++]["QuantityInCart"]));
            }

            return cartItems;
        }

        /// <summary>
        /// Get a list of books in the user's bookshelf.
        /// </summary>
        /// <returns>A list of books</returns>
        public List<Book> GetBookShelf()
        {
            return Book.GetBookShelf(this);
        }

        /// <summary>
        /// Check whether or not the user has bought the given book.
        /// </summary>
        /// <param name="BookID">the book's id</param>
        /// <returns>Whether or not the user has bought the given book</returns>
        public bool hasBought(int BookID)
        {
            return SQL.Execute("uspUserHasBoughtBook", new Param("UserID", UserID), new Param("BookID", BookID)).Rows.Count > 0;
        }

        /// <summary>
        /// Add the given book to the user's bookshelf.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void AddBookToBookShelf(int BookID)
        {
            SQL.Execute("uspAddBookToBookShelf", new Param("UserID", UserID), new Param("BookID", BookID));
        }

        /// <summary>
        /// Remove the given book from the user's bookshelf.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void RemoveBookFromBookShelf(int BookID)
        {
            SQL.Execute("uspRemoveBookFromBookShelf", new Param("UserID", UserID), new Param("BookID", BookID));
        }

        /// <summary>
        /// Add the given book to the user's cart.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void AddBookToCart(int BookID)
        {
            SQL.Execute("uspAddToCart", new Param("UserID", UserID), new Param("BookID", BookID));
        }

        /// <summary>
        /// Remove the given book from the user's cart.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void RemoveBookFromCart(int BookID)
        {
            SQL.Execute("uspRemoveFromCart", new Param("UserID", UserID), new Param("BookID", BookID));
        }

        /// <summary>
        /// Remove all books from the user's cart.
        /// </summary>
        public void EmptyCart()
        {
            SQL.Execute("uspEmptyCart", new Param("UserID", UserID));
        }

        /// <summary>
        /// Change the quantity of the given book in the user's cart.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        /// <param name="Quantity">The new quantity</param>
        public void UpdateQuantityInCart(int BookID, int Quantity)
        {
            SQL.Execute("uspUpdateQuantityInCart", new Param("UserID", UserID), new Param("BookID", BookID), new Param("Quantity", Quantity));
        }

        /// <summary>
        /// Get a list of all users.
        /// </summary>
        /// <returns>A list of all users</returns>
        public static List<User> GetUsers()
        {
            DataTable dt = SQL.Execute("uspGetUsers");
            List<User> users = new List<User>();
            foreach (DataRow dr in dt.Rows)
                users.Add(new User(
                    (int)dr["UserID"],
                    (String)dr["FirstName"],
                    (String)dr["LastName"],
                    (DateTime)dr["CreationDate"],
                    (String)dr["Password"],
                    (String)dr["Email"],
                    (Boolean)dr["IsDeleted"],
                    (Boolean)dr["IsAdmin"],
                    (Boolean)dr["IsConfirmed"],
                    (String)dr["ConfirmationCode"],
                    dr["Icon"] == DBNull.Value ? null : (byte[])dr["Icon"]));
            return users;
        }

        /// <summary>
        /// Get a list of all users from the given DataTable.
        /// </summary>
        /// <param name="dt">A DataTable containing users</param>
        /// <returns>A list of all users</returns>
        public static List<User> getUsers(DataTable dt)
        {
            List<User> users = new List<User>();
            foreach (DataRow dr in dt.Rows)
                users.Add(new User(
                    (int)dr["UserID"],
                    (String)dr["FirstName"],
                    (String)dr["LastName"],
                    (DateTime)dr["CreationDate"],
                    (String)dr["Password"],
                    (String)dr["Email"],
                    (Boolean)dr["IsDeleted"],
                    (Boolean)dr["IsAdmin"],
                    (Boolean)dr["IsConfirmed"],
                    (String)dr["ConfirmationCode"],
                    dr["Icon"] == DBNull.Value ? null : (byte[])dr["Icon"]));
            return users;
        }

        /// <summary>
        /// Check whether a user with the given email and password exists.
        /// </summary>
        /// <param name="Email">The user's email</param>
        /// <param name="Password">The user's password</param>
        /// <param name="id">The user's id to output if the user exists</param>
        /// <returns>Whether a user with the given email and password exists</returns>
        public static bool Exists(string Email, string Password, out int? id)
        {
            DataTable results = SQL.Execute("uspGetUserByEmailAndPassword",
                new Param("Email", Email),
                new Param("Password", Password));
            id = results.Rows.Count != 1 ? null : results.Rows[0]["UserID"] as int?;
            return results.Rows.Count >= 1;
        }

        /// <summary>
        /// Check whether a user with the given email and password exists.
        /// </summary>
        /// <param name="Email">The user's email</param>
        /// <param name="Password">The user's password</param>
        /// <returns>Whether a user with the given email and password exists</returns>
        public static bool Exists(string Email, string Password)
        {
            DataTable results = SQL.Execute("uspGetUserByEmailAndPassword",
                new Param("Email", Email),
                new Param("Password", Password));
            return results.Rows.Count >= 1;
        }

        /// <summary>
        /// Check whether a user with the given email exists.
        /// </summary>
        /// <param name="Email">The user's email</param>
        /// <returns>Whether a user with the given email exists</returns>
        public static bool Exists(string Email)
        {
            DataTable results = SQL.Execute("uspGetUserByEmail",
                new Param("Email", Email));
            return results.Rows.Count >= 1;
        }
    }
}
