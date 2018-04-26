using System;
using System.Data;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models the author of a book
    /// </summary>
    public class Author
    {
        /// <summary>
        /// The unique id number that identifies this author. -1 represents an uncreated author.
        /// </summary>
        public int AuthorID { private set; get; } = -1;
        /// <summary>
        /// The authors first name.
        /// </summary>
        public string FirstName;
        /// <summary>
        /// The authors last name.
        /// </summary>
        public string LastName;
        /// <summary>
        /// Whether or not the author has been deleted.
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// The first and last name of the author joined with a space
        /// </summary>
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        /// <summary>
        /// Instantiates a empty author with all of the default values.
        /// </summary>
        public Author() { }

        /// <summary>
        /// Instantiates an author and fills the fields with the author in the database with the given id number.
        /// </summary>
        /// <param name="AuthorID"></param>
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

        /// <summary>
        /// Instantiates and author and sets the fields to the given parameters.
        /// </summary>
        /// <param name="AuthorID">The author's id</param>
        /// <param name="FirstName">The author's first name</param>
        /// <param name="LastName">The author's last name</param>
        /// <param name="IsDeleted">Whether or not the author is deleted</param>
        private Author(int AuthorID, string FirstName, string LastName, bool IsDeleted)
        {
            this.AuthorID = AuthorID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.IsDeleted = IsDeleted;
        }

        /// <summary>
        /// Save the author to the database.
        /// </summary>
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

        //delete an author relationship from a book
        public void DeleteAuthorFromBook(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteAuthorFromBook",
                new Param("BookID", BookID),
                new Param("AuthorID", AuthorID)
                );
        }

        /// <summary>
        /// Delete all authors not related to a book
        /// </summary>
        public void DumpAuthors()
        {
            DataTable dt = SQL.Execute("uspDeleteEmptyAuthors");
        }

        /// <summary>
        /// Delete the author from the database and set the author's id to -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteAuthor", new Param("AuthorID", AuthorID));
            IsDeleted = true;
        }

        /// <summary>
        /// Add this author to a book.
        /// </summary>
        /// <param name="BookID"></param>
        public void AddAuthor(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddAuthor",
                new Param("AuthorID", AuthorID),
                new Param("BookID", BookID)
                );
        }

        /// <summary>
        /// Get a list of authors from a given book.
        /// </summary>
        /// <param name="BookID">The book's id number</param>
        /// <returns>A list of authors.</returns>
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

        /// <summary>
        /// Get a list of all authors.
        /// </summary>
        /// <returns>A list of all authors</returns>
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
