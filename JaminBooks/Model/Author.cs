﻿using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Author
    {
        public int AuthorID { private set; get; } = -1;
        public string FirstName;
        public string LastName;
        public bool IsDeleted;

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public Author() { }

        public Author(int AuthorID)
        {
            DataTable dt = SQL.Execute("uspGetAuthorByID", new Param("AuthorID", AuthorID));

            if (dt.Rows.Count > 0)
            {
                this.AuthorID = AuthorID;
                this.FirstName = (String)dt.Rows[0]["FirstName"];
                this.LastName = (String)dt.Rows[0]["LastName"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
            }
            else
            {
                throw new Exception("Invalid Author ID");
            }
        }

        private Author(int AuthorID, string FirstName, string LastName, bool IsDeleted)
        {
            this.AuthorID = AuthorID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.IsDeleted = IsDeleted;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveAuthor",
                new Param("AuthorID", AuthorID),
                new Param("FirstName", FirstName),
                new Param("LastName", LastName),
                new Param("IsDeleted", IsDeleted)
                );

            if (dt.Rows.Count > 0)
                AuthorID = (int)dt.Rows[0]["AuthorID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteAuthor", new Param("AuthorID", AuthorID));
            IsDeleted = true;
        }

        public void AddAuthor(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddAuthor",
                new Param("AuthorID", AuthorID),
                new Param("BookID", BookID)
                );
        }

        public static List<Author> GetAuthors(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetAuthors", new Param("BookID", BookID));
            List<Author> authors = new List<Author>();
            foreach (DataRow dr in dt.Rows)
                authors.Add(new Author(
                    (int)dr["AuthorID"],
                    (String)dr["FirstName"],
                    (String)dr["LastName"],
                    (bool)dr["IsDeleted"]
                    ));
            return authors;
        }

        public static List<Author> GetAuthors()
        {
            DataTable dt = SQL.Execute("uspGetAllAuthors");
            List<Author> authors = new List<Author>();
            foreach (DataRow dr in dt.Rows)
                authors.Add(new Author(
                    (int)dr["AuthorID"],
                    (String)dr["FirstName"],
                    (String)dr["LastName"],
                    (bool)dr["IsDeleted"]
                    ));
            return authors;
        }


    }
}
