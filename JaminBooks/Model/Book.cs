using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Book
    {
        public int BookID { private set; get; } = -1;

        public string Title;
        public int AuthorID;
        public DateTime PublicationDate;
        public int PublisherID;
        public int ISBN10;
        public int ISBN13;
        public string Description;
        public DateTime CopyrightData;
        public decimal Price;
        public decimal Cost;
        public int Quantity;
        public bool IsDeleted;

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
                this.ISBN10 = (int)dt.Rows[0]["ISBN10"];
                this.ISBN13 = (int)dt.Rows[0]["ISBN13"];
                this.Description = (String)dt.Rows[0]["Description"];
                this.CopyrightData = (DateTime)dt.Rows[0]["CopyrightData"];
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
            DataTable dt = SQL.Execute("uspSaveBook",
                new Param("BookID", BookID),
                new Param("AuthorID", AuthorID),
                new Param("PublicationDate", PublicationDate),
                new Param("PublisherID", PublisherID),
                new Param("ISBN10", ISBN10),
                new Param("ISBN13", ISBN13),
                new Param("Description", Description),
                new Param("Price", Price),
                new Param("Cost", Cost),
                new Param("Quantity", Quantity));

            if (dt.Rows.Count > 0)
                BookID = (int)dt.Rows[0]["BookID"];
        }
    }
}