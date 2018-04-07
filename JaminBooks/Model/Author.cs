using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Author
    {
        public int BookID { private set; get; } = -1;

        public int AuthorID;
        public string AFirstName;
        public string ALastName;     

        public Author() { }

        public Author(int AuthorID)
        {
            DataTable dt = SQL.Execute("uspGetAuthorByID", new Param("AuthorID", AuthorID));
            if (dt.Rows.Count > 0)
            {
                this.AuthorID = AuthorID;
                this.AFirstName = (string)dt.Rows[0]["AFirstName"]; ;
                this.ALastName = (string)dt.Rows[0]["ALastName"]; ;
            }
            else
            {
                throw new Exception("Invalid ID");
            }
        }


        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveAuthor",
            new Param("FirstName", AFirstName),
            new Param("LastName", ALastName));
            if (dt.Rows.Count > 0)
            {
                AuthorID = (int)dt.Rows[0]["AuthorID"];
            }
            else
            {
                throw new Exception("Book Not Created");
            }
        }

        public void delete()
        {
            DataTable dt = SQL.Execute("uspDeleteAuthor",
                new Param("AuthorID", AuthorID));
                AuthorID = -1;

        }

        public int GetAuthorIDByName()
        {
            DataTable dt = SQL.Execute("uspGetAuthorIDByName",
                new Param("FirstName", AFirstName),
                new Param("LastName", ALastName));
            if (dt.Rows.Count > 0)
                AuthorID = (int)dt.Rows[0]["AuthorID"];
            else
            {
                Save();
            }
            return AuthorID;
        }

    }
}