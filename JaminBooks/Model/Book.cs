using System;
using System.Data;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a book
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The unique id number that identifies this book. -1 represents an uncreated book.
        /// </summary>
        public int BookID { private set; get; } = -1;

        /// <summary>
        /// The title of the book.
        /// </summary>
        public string Title;
        /// <summary>
        /// The id number of this book's publisher.
        /// </summary>
        public int PublisherID = -1;
        /// <summary>
        /// The date of publication.
        /// </summary>
        public DateTime PublicationDate;
        /// <summary>
        /// This book's ISBN 10 number.
        /// </summary>
        public string ISBN10;
        /// <summary>
        /// This book's ISBN 13 number.
        /// </summary>
        public string ISBN13;
        /// <summary>
        /// This book's description.
        /// </summary>
        public string Description;
        /// <summary>
        /// The copyright date.
        /// </summary>
        public DateTime CopyrightDate;
        /// <summary>
        /// This book's original price.
        /// </summary>
        public decimal _Price;
        /// <summary>
        /// The cost of this book.
        /// </summary>
        public decimal Cost;
        /// <summary>
        /// The number of units of this book in stock.
        /// </summary>
        public int Quantity;
        /// <summary>
        /// Whether or not this book has been deleted.
        /// </summary>
        public bool IsDeleted = false;
        /// <summary>
        /// A byte representation of this book's image.
        /// </summary>
        public byte[] BookImage { set; private get; }
        /// <summary>
        /// This books star rating as an integer. Should be between 0 and 5.
        /// </summary>
        public int Rating;
        /// <summary>
        /// The best discount currently on this book.
        /// </summary>
        public int PercentDiscount = 0;
        /// <summary>
        /// Whether or not to load the full publisher of this book or just the name of the publisher.
        /// </summary>
        public bool LoadPublisher = true;

        /// <summary>
        /// The discounted price of this book.
        /// </summary>
        public decimal Price
        {
            get
            {
                return Math.Round((_Price - (_Price * (PercentDiscount / 100m))), 2);
            }
            set
            {
                _Price = value;
            }
        }

        /// <summary>
        /// Whether or not this book has an image.
        /// </summary>
        public bool HasIcon
        {
            get
            {
                return BookImage != null;
            }
        }

        /// <summary>
        /// Cache this book's image if the image has not yet been cached. If the image has been cached, return the path to the image. 
        /// </summary>
        public string LoadImage
        {
            get
            {
                if (BookImage.Count() < 10) return "/images/missing.png";
                using (MemoryStream ms = new MemoryStream(BookImage))
                {
                    var filename = Authentication.Hash(Convert.ToBase64String(BookImage)) + ".png";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "temp");
                    Directory.CreateDirectory(path);

                    path = Path.Combine(path, filename);

                    if (!File.Exists(path))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fs);
                            fs.Flush();
                        }
                    }

                    return "/images/temp/" + filename;
                }
            }
        }

        /// <summary>
        /// A list of all the categories this book is under.
        /// </summary>
        public List<Category> Categories
        {
            get
            {
                return Category.GetCategories(this.BookID);
            }
        }

        /// <summary>
        /// A list of all this books authors.
        /// </summary>
        public List<Author> Authors
        {
            get
            {
                return Author.GetAuthors(this.BookID);
            }
        }

        /// <summary>
        /// An integer representing the total number of sales of this book.
        /// </summary>
        public int Sales
        {
            get
            {
                return (int)SQL.Execute("uspGetSalesByBook", new Param("BookID", this.BookID)).Rows[0]["Sales"];
            }
        }

        /// <summary>
        /// The publisher of this book.
        /// </summary>
        public Publisher Publisher
        {
            get
            {
                if (LoadPublisher)
                    return new Publisher(PublisherID);
                else
                {
                    Publisher p = new Publisher();
                    p.PublisherName = new Publisher(PublisherID).PublisherName;
                    return p;
                }
            }
        }

        /// <summary>
        /// Instantiates an empty book with all of the default values.
        /// </summary>
        public Book() { }

        /// <summary>
        /// Instantiates a book and fills the fields with the book in the database with the given id number.
        /// </summary>
        /// <param name="BookID">The books id number</param>
        public Book(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetBookByID", new Param("BookID", BookID));

            if (dt.Rows.Count > 0)
            {
                this.BookID = BookID;
                this.Title = (string)dt.Rows[0]["Title"];
                this.PublicationDate = (DateTime)dt.Rows[0]["PublicationDate"];
                this.PublisherID = (int)dt.Rows[0]["PublisherID"];
                this.ISBN10 = (string)dt.Rows[0]["ISBN10"];
                this.ISBN13 = (string)dt.Rows[0]["ISBN13"];
                this.Description = (string)dt.Rows[0]["Description"];
                this.CopyrightDate = (DateTime)dt.Rows[0]["CopyrightDate"];
                this._Price = (decimal)dt.Rows[0]["Price"];
                this.Cost = (decimal)dt.Rows[0]["Cost"];
                this.Quantity = (int)dt.Rows[0]["Quantity"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
                this.BookImage = dt.Rows[0]["BookImage"] == DBNull.Value ? null : (byte[])dt.Rows[0]["BookImage"];
                var rating = SQL.Execute("uspGetAverageRating", new Param("BookID", BookID)).Rows[0]["Rating"];
                this.Rating = rating != DBNull.Value ? Convert.ToInt32(rating) : 0;
                this.PercentDiscount = Promotions.GetDiscount(this);
            }
            else
            {
                throw new Exception("Invalid Book ID");
            }
        }

        /// <summary>
        /// Instantiates a book with all of the fields set to the value of the given parameters
        /// </summary>
        /// <param name="BookID">The book's id</param>
        /// <param name="Title">The book's title</param>
        /// <param name="PublicationDate">The book's publication date</param>
        /// <param name="PublisherID">The id of the book's publisher</param>
        /// <param name="ISBN10">The ISBN 10 of this book</param>
        /// <param name="ISBN13">The ISBN 13 of the book</param>
        /// <param name="Description">The book's description</param>
        /// <param name="CopyrightDate">the book's copyright date</param>
        /// <param name="_Price">The book's non-discounted price</param>
        /// <param name="Cost">The book's cost</param>
        /// <param name="Quantity">The book's quantity in stock</param>
        /// <param name="IsDeleted">Whether or not the book is deleted</param>
        /// <param name="BookImage">The books image</param>
        private Book(int BookID, string Title, DateTime PublicationDate, int PublisherID, string ISBN10,
            string ISBN13, string Description, DateTime CopyrightDate, decimal _Price, decimal Cost,
            int Quantity, bool IsDeleted, byte[] BookImage)
        {
            this.BookID = BookID;
            this.Title = Title;
            this.PublicationDate = PublicationDate;
            this.PublisherID = PublisherID;
            this.ISBN10 = ISBN10;
            this.ISBN13 = ISBN13;
            this.Description = Description;
            this.CopyrightDate = CopyrightDate;
            this._Price = _Price;
            this.Cost = Cost;
            this.Quantity = Quantity;
            this.IsDeleted = IsDeleted;
            this.BookImage = BookImage;
            var rating = SQL.Execute("uspGetAverageRating", new Param("BookID", BookID)).Rows[0]["Rating"];
            this.Rating = rating != DBNull.Value ? Convert.ToInt32(rating) : 0;
            this.PercentDiscount = Promotions.GetDiscount(this);
        }

        /// <summary>
        /// Save the book to the database
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveBook",
                new Param("Title", Title),
                new Param("BookID", BookID),
                new Param("PublicationDate", PublicationDate),
                new Param("PublisherID", PublisherID),
                new Param("ISBN10", ISBN10),
                new Param("ISBN13", ISBN13),
                new Param("Description", Description),
                new Param("CopyrightDate", CopyrightDate),
                new Param("Price", _Price),
                new Param("Cost", Cost),
                new Param("Quantity", Quantity),
                new Param("IsDeleted", IsDeleted),
                new Param("BookImage", BookImage ?? SqlBinary.Null));

            if (dt.Rows.Count > 0)
                BookID = (int)dt.Rows[0]["BookID"];
        }

        /// <summary>
        /// Delete the book in the database and set its id to -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteBook", new Param("BookID", BookID));
            BookID = -1;

        }

        /// <summary>
        /// Add a publisher to this book
        /// </summary>
        /// <param name="p">The publisher</param>
        public void AddPublisher(Publisher p)
        {
            p.AddPublisher(this.BookID);
        }

        /// <summary>
        /// Add an author to this book.
        /// </summary>
        /// <param name="a">The author</param>
        public void AddAuthor(Author a)
        {
            a.AddAuthor(this.BookID);
        }

        /// <summary>
        /// Add a category to this book.
        /// </summary>
        /// <param name="c">The category</param>
        public void AddCategory(Category c)
        {
            c.AddCategory(this.BookID);
        }

        /// <summary>
        /// Get a list of books on a user's bookshelf.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>A list of books on the bookshelf.</returns>
        public static List<Book> GetBookShelf(User user)
        {
            return GetBooks(SQL.Execute("uspGetBookShelf", new Param("UserID", user.UserID)));
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>A list of all books</returns>
        public static List<Book> GetBooks()
        {
            DataTable dt = SQL.Execute("uspGetBooks");
            List<Book> books = new List<Book>();
            foreach (DataRow dr in dt.Rows)
                books.Add(new Book(
                    (int)dr["BookID"],
                    (string)dr["Title"],
                    (DateTime)dr["PublicationDate"],
                    (int)dr["PublisherID"],
                    (String)dr["ISBN10"],
                    (String)dr["ISBN13"],
                    (String)dr["Description"],
                    (DateTime)dr["CopyrightDate"],
                    (decimal)dr["Price"],
                    (decimal)dr["Cost"],
                    (int)dr["Quantity"],
                    (bool)dr["IsDeleted"],
                    (dr["BookImage"] != DBNull.Value ? (byte[])dr["BookImage"] : new byte[1])));
            return books;
        }

        /// <summary>
        /// Get a list of books from the given DataTable.
        /// </summary>
        /// <param name="dt">A DataTable containing books</param>
        /// <returns>A list of books.</returns>
        public static List<Book> GetBooks(DataTable dt)
        {
            List<Book> books = new List<Book>();
            foreach (DataRow dr in dt.Rows)
                books.Add(new Book(
                    (int)dr["BookID"],
                    (string)dr["Title"],
                    (DateTime)dr["PublicationDate"],
                    (int)dr["PublisherID"],
                    (String)dr["ISBN10"],
                    (String)dr["ISBN13"],
                    (String)dr["Description"],
                    (DateTime)dr["CopyrightDate"],
                    (decimal)dr["Price"],
                    (decimal)dr["Cost"],
                    (int)dr["Quantity"],
                    (bool)dr["IsDeleted"],
                    (dr["BookImage"] != DBNull.Value ? (byte[])dr["BookImage"] : new byte[1])));
            return books;
        }
    }
}
