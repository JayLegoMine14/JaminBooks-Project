using JaminBooks.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Tools
{
    /// <summary>
    /// Manages Book, Code, and Total promotions.
    /// </summary>
    public class Promotions
    {
        /// <summary>
        /// Get the best discount on the given book.
        /// </summary>
        /// <param name="b">The book</param>
        /// <returns>The best discount</returns>
        public static int GetDiscount(Book b)
        {
            return GetDiscount(SQL.Execute("uspGetBookDiscount", new Param("BookID", b.BookID)));
        }

        /// <summary>
        /// Get the promotion currently on the given book.
        /// </summary>
        /// <param name="b">The book</param>
        /// <returns>The promotion on the book</returns>
        public static Promotion GetPromotion(Book b)
        {
            DataTable results = SQL.Execute("uspGetPromotionByBook", new Param("BookID", b.BookID));
            if (results.Rows.Count > 0)
                return new Promotion((int)results.Rows[0]["PromotionID"]);
            else return null;
        }

        /// <summary>
        /// Get the best discount given the order total.
        /// </summary>
        /// <param name="total">The order total</param>
        /// <returns>THe best discount</returns>
        public static int GetDiscount(decimal total)
        {
            return GetDiscount(SQL.Execute("uspGetTotalDiscount", new Param("Total", total)));
        }

        /// <summary>
        /// Gets the best discount from the given code.
        /// </summary>
        /// <param name="code">The discount code</param>
        /// <returns>The best discount</returns>
        public static int GetDiscount(string code)
        {
            return GetDiscount(SQL.Execute("uspGetCodeDiscount", new Param("Code", code)));
        }

        /// <summary>
        /// Get the discount from the given DataTable.
        /// </summary>
        /// <param name="results">A DataTable containing a discount</param>
        /// <returns>The discount</returns>
        private static int GetDiscount(DataTable results)
        {
            if (results.Rows.Count > 0)
                return (int)results.Rows[0]["PercentDiscount"];
            else return 0;
        }
    }
}
