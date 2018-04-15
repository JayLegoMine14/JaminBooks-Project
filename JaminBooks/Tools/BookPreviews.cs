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
        public static Book GetBestSeller()
        {
            return new Book((int)SQL.Execute("uspGetBestSeller").Rows[0]["BookID"]);
        }

        public static Book GetMostPopular()
        {
            return new Book((int)SQL.Execute("uspGetMostPopular").Rows[0]["BookID"]);
        }

        public static Book GetReccomended(User u)
        {
            if (u != null)
            {
                DataTable results = SQL.Execute("uspGetReccomended", new Param("UserID", u.UserID));
                if(results.Rows.Count > 0)
                {
                    return new Book((int)results.Rows[0]["BookID"]);
                }
                else return GetMostPopular();
            }
            else return GetMostPopular();
        }
    }
}
