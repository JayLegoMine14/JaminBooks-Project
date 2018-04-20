using JaminBooks.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Tools
{
    public class BookPreviews
    {
        public static List<Book> GetBestSellers()
        {
            return Book.GetBooks(SQL.Execute("uspGetAlltimeBestSellers"));
        }

        public static List<Book> GetMostPopular()
        {
            return Book.GetBooks(SQL.Execute("uspGetMostPopular"));
        }

        public static List<Book> GetSales()
        {
            return Book.GetBooks(SQL.Execute("uspGetSales"));
        }

        public static Book GetReccomended(User u)
        {
            if (u != null)
            {
                DataTable results = SQL.Execute("uspGetReccomended", new Param("UserID", u.UserID));
                if (results.Rows.Count > 0)
                {
                    return new Book((int)results.Rows[0]["BookID"]);
                }
                else return GetMostPopular()[0];
            }
            else return GetMostPopular()[0];
        }
    }
}
