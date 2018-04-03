﻿using System;
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
        public string ISBN10;
        public string ISBN13;
        public string Description;
        public string CategoryID;
        public DateTime CopyrightDate;
        public decimal Price;
        public decimal Cost;
        public int Quantity;
        public bool IsDeleted;
        public string CategoryName;
        public int TempVal;

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

        public string GetCategoryIDByName(string CategoryName)
        {
            DataTable dt = SQL.Execute("uspGetCategoryIDByName",
                new Param("CategoryName", CategoryName));

            if (dt.Rows.Count == 0)
            {
              CategoryID = SaveCategoryName(CategoryName);
            }
            else
            {
                TempVal = (int)dt.Rows[0]["CategoryID"];
                CategoryID = TempVal.ToString();
            }


            return CategoryID;
        }

        public string SaveCategoryName(string CategoryName)
        {
           
            DataTable dt = SQL.Execute("uspSaveCategoryName",
            new Param("CategoryName", CategoryName));
            if (dt.Rows.Count > 0)
            {
                TempVal = (int)dt.Rows[0]["CategoryID"];
                CategoryID = TempVal.ToString();
            } else
            {
                throw new Exception("Book Not Created");
            }
           
            return CategoryID;
        }

        public void Save()
        {

            GetCategoryIDByName(CategoryName);

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

<<<<<<< HEAD
        public void delete(int BookID)
=======
        public void Delete(int BookID)
>>>>>>> master
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