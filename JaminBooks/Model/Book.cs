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
<<<<<<< HEAD
        public int AuthorID;    
=======
        public string AuthorID;    
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        public DateTime PublicationDate;
        public string PublisherID;
        public string ISBN10;
        public string ISBN13;
        public string Description;
<<<<<<< HEAD
        public int CategoryID;
=======
        public string CategoryID;
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        public DateTime CopyrightDate;
        public decimal Price;
        public decimal Cost;
        public int Quantity;
        public bool IsDeleted;
        public string CategoryName;
        public int TempVal;
        public string AFirstName;
        public string ALastName;
        public string AuthorName;
        public string PublisherName;

        public Book() { }

        public Book(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetBookByID", new Param("BookID", BookID));
            if (dt.Rows.Count > 0)
            {
                this.BookID = BookID;
<<<<<<< HEAD
                this.AuthorID = (int)dt.Rows[0]["AuthorID"];
                this.PublicationDate = (DateTime)dt.Rows[0]["PublicationDate"];
                this.PublisherID = (int)dt.Rows[0]["PublisherID"];
=======
                this.AuthorID = (string)dt.Rows[0]["AuthorID"];
                this.PublicationDate = (DateTime)dt.Rows[0]["PublicationDate"];
                this.PublisherID = (string)dt.Rows[0]["PublisherID"];
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
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

<<<<<<< HEAD
        public int GetAuthorIDByName(string AuthorName)
=======
        public string GetAuthorIDByName(string AuthorName)
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        {
            DataTable dt = SQL.Execute("uspGetAuthorIDByName",
                new Param("FirstName", AFirstName),
                new Param("FirstName", ALastName));

            if (dt.Rows.Count == 0)
            {
                AuthorID = SaveAuthorName(AFirstName, ALastName);
            }
            else
            {
                TempVal = (int)dt.Rows[0]["AuthorID"];
                AuthorName = TempVal.ToString();
            }


            return AuthorID;
        }

<<<<<<< HEAD
        public int SaveAuthorName(string AFirstName, string ALastName)
=======
        public string SaveAuthorName(string AFirstName, string ALastName)
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        {

            DataTable dt = SQL.Execute("uspSaveAuthorName",
            new Param("AuthorName", AFirstName),
            new Param("LastName", ALastName));
            if (dt.Rows.Count > 0)
            {
<<<<<<< HEAD
                AuthorID = (int)dt.Rows[0]["AuthorID"];

=======
                TempVal = (int)dt.Rows[0]["AuthorID"];
                AuthorID = TempVal.ToString();
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
            }
            else
            {
                throw new Exception("Book Not Created");
            }

            return AuthorID;
        }

<<<<<<< HEAD
        public int GetCategoryIDByName(string CategoryName)
=======
        public string GetCategoryIDByName(string CategoryName)
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        {
            DataTable dt = SQL.Execute("uspGetCategoryIDByName",
                new Param("CategoryName", CategoryName));

            if (dt.Rows.Count == 0)
            {
              CategoryID = SaveCategoryName(CategoryName);
            }
            else
            {
<<<<<<< HEAD
                CategoryID = (int)dt.Rows[0]["CategoryID"];
                
=======
                TempVal = (int)dt.Rows[0]["CategoryID"];
                CategoryID = TempVal.ToString();
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
            }


            return CategoryID;
        }

<<<<<<< HEAD
        public int SaveCategoryName(string CategoryName)
=======
        public string SaveCategoryName(string CategoryName)
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
        {
           
            DataTable dt = SQL.Execute("uspSaveCategoryName",
            new Param("CategoryName", CategoryName));
            if (dt.Rows.Count > 0)
            {
<<<<<<< HEAD
                CategoryID = (int)dt.Rows[0]["CategoryID"];

=======
                TempVal = (int)dt.Rows[0]["CategoryID"];
                CategoryID = TempVal.ToString();
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5
            } else
            {
                throw new Exception("Book Not Created");
            }
           
            return CategoryID;
        }

        public string SavePublisherName(string PublisherName)
        {
            {

                DataTable dt = SQL.Execute("uspSavePublisherName",
                new Param("PublisherName", PublisherName));
                if (dt.Rows.Count > 0)
                {
                    TempVal = (int)dt.Rows[0]["PublisherID"];
                    PublisherID = TempVal.ToString();
                }
                else
                {
                    throw new Exception("Book Not Created");
                }

                return PublisherID;
            }
        }

<<<<<<< HEAD
        

        public void Save()
        {
            Publisher pub = new Publisher();
            Author auth = new Author();


            GetCategoryIDByName(CategoryName);
            auth.GetAuthorIDByName(AFirstName, ALastName);
            pub.GetPublisherIDByName(PublisherName);
=======
        public string GetPublisherIDByName(string PublisherName)
        {
            DataTable dt = SQL.Execute("uspGetPublisherIDByName",
                new Param("PublisherName", PublisherName));

            if (dt.Rows.Count == 0)
            {
                PublisherID = SavePublisherName(PublisherName);
            }
            else
            {
                TempVal = (int)dt.Rows[0]["PublisherID"];
                PublisherID = TempVal.ToString();
            }


            return PublisherID;
        }

        public void Save()
        {

            GetCategoryIDByName(CategoryName);
            GetAuthorIDByName(AuthorName);
            GetPublisherIDByName(PublisherName);
>>>>>>> 75b3758e4b0f548656306774cf66448fa52094a5

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
                new Param("Quantity", Quantity));

                if (dt.Rows.Count > 0)
                    BookID = (int)dt.Rows[0]["BookID"];
                else
                {
                    throw new Exception("Invalid Entry");
                }
                        
        }



        public void Delete(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteBook",
                new Param("BookID", BookID));
            if (dt.Rows.Count > 0)
                IsDeleted = true;
            else
            {
                throw new Exception("Invalid  ID");
            }

        }
    }
}