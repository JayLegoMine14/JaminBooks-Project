using JaminBooks.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Tools
{
    public class Promotions
    {
        public static int GetDiscount(Book b)
        {
            return GetDiscount(SQL.Execute("uspGetBookDiscount", new Param("BookID", b.BookID)));
        }

        public static Promotion GetPromotion(Book b)
        {
            DataTable results = SQL.Execute("uspGetPromotionByBook", new Param("BookID", b.BookID));
            if (results.Rows.Count > 0)
                return new Promotion((int)results.Rows[0]["PromotionID"]);
            else return null;
        }

        public static int GetDiscount(decimal total)
        {
            return GetDiscount(SQL.Execute("uspGetTotalDiscount", new Param("Total", total)));
        }

        public static int GetDiscount(string code)
        {
            return GetDiscount(SQL.Execute("uspGetCodeDiscount", new Param("Code", code)));
        }

        private static int GetDiscount(DataTable results)
        {
            if (results.Rows.Count > 0)
                return (int)results.Rows[0]["PercentDiscount"];
            else return 0;
        }
    }
}
