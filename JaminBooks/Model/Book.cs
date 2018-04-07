using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Book
    {
        public int BookID { private set; get; } = -1;

        //Book Fields
        public string Title;   
        public DateTime PublicationDate;
        public string ISBN10;
        public string ISBN13;
        public string Description;
        public DateTime CopyrightDate;
        public decimal Price;
        public decimal Cost;
        public int Quantity;
        public bool IsDeleted;
        public string CategoryName;
        public byte[] BookImage;

        //book ID prerequisites
        public int AuthorID;
        public int PublisherID;
        public int CategoryID;

        //Author Fields
        public string AFirstName;
        public string ALastName;
        
        //publusher fields
        public string PublisherName;
        public string ContactFirstName;
        public string ContactLastName;

        //Publisher requisites
        public int AddressID;
        public int PhoneID;

        //Address Fields
        public string Line1;
        public string Line2;
        public string City;
        public string State;
        public string Country;
        public string ZIP;

        //Phone Fields
        public string Number;
        public string PhoneCategory;

        public Book() { }

        public Book(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetBookByID", new Param("BookID", BookID));
            if (dt.Rows.Count > 0)
            {
                this.BookID = BookID;
                this.AuthorID = (int)dt.Rows[0]["AuthorID"];
                this.PublicationDate = (DateTime)dt.Rows[0]["PublicationDate"];
                this.PublisherID = (int)dt.Rows[0]["PublisherID"];
                this.ISBN10 = (string)dt.Rows[0]["ISBN10"];
                this.ISBN13 = (string)dt.Rows[0]["ISBN13"];
                this.Description = (String)dt.Rows[0]["Description"];
                this.CategoryName = (String)dt.Rows[0]["CategoryName"];
                this.CopyrightDate = (DateTime)dt.Rows[0]["CopyrightDate"];
                this.Price = (Decimal)dt.Rows[0]["Price"];
                this.Cost = (Decimal)dt.Rows[0]["Cost"];
                this.Quantity = (int)dt.Rows[0]["Quantity"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
            }
            else
            {
                throw new Exception("Invalid User ID");
            }
        }

        

        public void Save()
        {
            Publisher Publisher = new Publisher();
            Author Author = new Author();
            Category Category = new Category();

            Publisher.PublisherName = PublisherName;
            Publisher.AddressID = AddressID;
            Publisher.PhoneID = PhoneID;
            Publisher.ContactFirstName = ContactFirstName;
            Publisher.ContactLastName = ContactLastName;

            //address 
            Publisher.Line1 = Line1;
            Publisher.Line2 = Line2;
            Publisher.City = City;
            Publisher.State = State;
            Publisher.Country = Country;
            Publisher.ZIP = ZIP;

            //phone 
            Publisher.Number = Number;
            Publisher.Category = PhoneCategory;

            //author
            Author.AFirstName = AFirstName;
            Author.ALastName = ALastName;

            Category.CategoryName = CategoryName;


            Publisher.Save();
            Category.Save();
            
                     
           CategoryID = Category.GetCategoryIDByName();
           AuthorID = Author.GetAuthorIDByName();
           PublisherID = Publisher.GetPublisherIDByName();

            

            DataTable dt = SQL.Execute("uspSaveBook",
            new Param("Title", Title),
            new Param("BookID", BookID),
            new Param("AuthorID", AuthorID),
            new Param("PublicationDate", PublicationDate),
            new Param("PublisherID", PublisherID),
            new Param("ISBN10", ISBN10),
            new Param("ISBN13", ISBN13),
            new Param("Description", Description),
            new Param("CategoryID", CategoryID),
            new Param("CopyrightDate", CopyrightDate),
            new Param("Price", Price),
            new Param("Cost", Cost),
            new Param("Quantity", Quantity),
            new Param("BookImage", BookImage ?? SqlBinary.Null));

            if (dt.Rows.Count > 0)
                    BookID = (int)dt.Rows[0]["BookID"];
                else
                {
                    throw new Exception("Invalid Entry");
                }

             Category.SaveCategoryToBook(BookID);           
        }



        public void Delete(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteBook",
                new Param("BookID", BookID));
                IsDeleted = true;
        }

        
    }
}