using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    public class Book
    {
        public int BookID { private set; get; } = -1;

        public string Title;
        public int AuthorID = -1;
        public int PublisherID = -1;
        public DateTime PublicationDate;
        public string ISBN10;
        public string ISBN13;
        public string Description;
        public DateTime CopyrightDate;
        public decimal Price;
        public decimal Cost;
        public int Quantity;
        public bool IsDeleted = false;
        public byte[] BookImage { set; private get; }
        public int Rating;

        public bool LoadPublisher = true;

        public bool HasIcon
        {
            get
            {
                return BookImage != null;
            }
        }

        //loads the book image
        public string LoadImage
        {
            get
            {
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

        //creates the list of categories for a book
        public List<Category> Categories
        {
            get
            {
                return Category.GetCategories(this.BookID);
            }
        }

        //creates the list of authors for a book
        public List<Author> Authors
        {
            get
            {
                return Author.GetAuthors(this.BookID);
            }
        }

        //returns the sales for a book
        public int Sales
        {
            get
            {
                return (int)SQL.Execute("uspGetSalesByBook", new Param("BookID", this.BookID)).Rows[0]["Sales"];
            }
        }

        //Creates a publisher within a book
        public Publisher Publisher
        {
            get
            {
                if(LoadPublisher)
                    return new Publisher(PublisherID);
                else
                {
                    Publisher p = new Publisher();
                    p.PublisherName = new Publisher(PublisherID).PublisherName;
                    return p;
                }
            }
        }

        //creates a list of publishers
        public List<Publisher> Publishers
        {
            get
            {
                return Publisher.GetPublishers(this.BookID);
            }
        }
        
        //book constructor
        public Book() { }

        //data table book constructor
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
                this.Price = (decimal)dt.Rows[0]["Price"];
                this.Cost = (decimal)dt.Rows[0]["Cost"];
                this.Quantity = (int)dt.Rows[0]["Quantity"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
                this.BookImage = dt.Rows[0]["BookImage"] == DBNull.Value ? null : (byte[])dt.Rows[0]["BookImage"];
                var rating = SQL.Execute("uspGetAverageRating", new Param("BookID", BookID)).Rows[0]["Rating"];
                this.Rating = rating != DBNull.Value ? Convert.ToInt32(rating) : 0;
            }
            else
            {
                throw new Exception("Invalid Book ID");
            }
        }

        //private book contructor
        private Book(int BookID, string Title, DateTime PublicationDate, int PublisherID, string ISBN10,
            string ISBN13, string Description, DateTime CopyrightDate, decimal Price, decimal Cost,
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
            this.Price = Price;
            this.Cost = Cost;
            this.Quantity = Quantity;
            this.IsDeleted = IsDeleted;
            this.BookImage = BookImage;
            var rating = SQL.Execute("uspGetAverageRating", new Param("BookID", BookID)).Rows[0]["Rating"];
            this.Rating = rating != DBNull.Value ? Convert.ToInt32(rating) : 0;
        }

        //saves a book to the database
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
                new Param("Price", Price),
                new Param("Cost", Cost),
                new Param("Quantity", Quantity),
                new Param("IsDeleted", IsDeleted),
                new Param("BookImage", BookImage ?? SqlBinary.Null));

            if (dt.Rows.Count > 0)
                BookID = (int)dt.Rows[0]["BookID"];
        }

        //sets the "isdeleted" value of a book to true without completely deleting it
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteBook", new Param("BookID", BookID));
            BookID = -1;

        }

        //adds a publisher to a book
        public void AddPublisher(Publisher p)
        {
            p.AddPublisher(this.BookID);
        }

        //adds an author to a book
        public void AddAuthor(Author a)
        {
            a.AddAuthor(this.BookID);
        }

        //adds a category to a book
        public void AddCategory(Category c)
        {
            c.AddCategory(this.BookID);
        }

        //returns the list of books from the database
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

        //also returns a list of books but stores in the object
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
                    (dr["BookImage"] != DBNull.Value ? (byte[]) dr["BookImage"] : new byte[1])));
            return books;
        }
    }
}
