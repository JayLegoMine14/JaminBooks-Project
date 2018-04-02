using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Publisher
    {
        public int BookID { private set; get; } = -1;

        public int PublisherID;

       

        public Publisher() { }

        public Publisher(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetBookByID", new Param("BookID", BookID));
            if (dt.Rows.Count > 0)
            {
                
            }
            else
            {
                throw new Exception("");
            }
        }

        public void Save()
        {

            DataTable dt = SQL.Execute("uspGetPblisher", new Param("PublisherID",PublisherID ));
            if (dt.Rows.Count > 0)
                BookID = (int)dt.Rows[0]["BookID"];
            else
            {
                throw new Exception("");
            }
        }

        public void delete()
        {
            DataTable dt = SQL.Execute("uspDeleteBook",
                new Param("BookID", BookID));
            if (dt.Rows.Count > 0)
            {

            }
                
            else
            {
                throw new Exception("Invalid  ID");
            }

        }
    }
}