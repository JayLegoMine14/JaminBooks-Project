using JaminBooks.Model;
using System.Collections.Generic;
using System.Data;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Tools
{
    /// <summary>
    /// Manages list of books that meet certain criteria
    /// </summary>
    public class BookPreviews
    {
        /// <summary>
        /// Get a list of books that have sold the best over the history of the store.
        /// </summary>
        /// <returns>A list of books</returns>
        public static List<Book> GetBestSellers()
        {
            return Book.GetBooks(SQL.Execute("uspGetAlltimeBestSellers"));
        }

        /// <summary>
        /// Get a list of the most popular books based on comments and ratings.
        /// </summary>
        /// <returns>A list of books</returns>
        public static List<Book> GetMostPopular()
        {
            return Book.GetBooks(SQL.Execute("uspGetMostPopular"));
        }

        /// <summary>
        /// Get a list of books that are on sales.
        /// </summary>
        /// <returns>A list of books</returns>
        public static List<Book> GetSales()
        {
            return Book.GetBooks(SQL.Execute("uspGetSales"));
        }

        /// <summary>
        /// Get a list of books that are recommended for the current user based on past purchases
        /// </summary>
        /// <param name="u">The user</param>
        /// <returns>A list of books</returns>
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