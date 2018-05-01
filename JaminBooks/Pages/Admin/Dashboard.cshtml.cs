using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Displays a dashboard with data about sales and users over the given period.
    /// </summary>
    public class DashboardModel : PageModel
    {
        /// <summary>
        /// The user currently logged in.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// The start date of the period.
        /// </summary>
        public DateTime Start;

        /// <summary>
        /// The end date of the period.
        /// </summary>
        public DateTime End;

        /// <summary>
        /// The number of ratings flagged.
        /// </summary>
        public int FlaggedRatings;

        /// <summary>
        /// THe number of order reshipped.
        /// </summary>
        public int ReshippedOrders;

        /// <summary>
        /// A list of fields and their values.
        /// </summary>
        public Dictionary<string, object> Fields;

        /// <summary>
        /// A list of best selling books.
        /// </summary>
        public Dictionary<Book, int> BestSellers = new Dictionary<Book, int>();

        /// <summary>
        /// A list of worst selling books.
        /// </summary>
        public Dictionary<Book, int> WorstSellers = new Dictionary<Book, int>();

        /// <summary>
        /// Load the page on a post request. Runs when the period has changed and the load button is clicked.
        /// </summary>
        public void OnPost()
        {
            Start = DateTime.Parse(Request.Form["start"]);
            End = DateTime.Parse(Request.Form["end"]);
            RenderPage(Start, End);
        }

        /// <summary>
        /// Load the page on a get request.
        /// </summary>
        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            Start = DateTime.Now.AddDays(-7);
            End = DateTime.Now;
            RenderPage(Start, End);
        }

        /// <summary>
        /// Render the page with the given start and end dates.
        /// </summary>
        public void RenderPage(DateTime Start, DateTime end)
        {
            FlaggedRatings = Rating.GetFlagged().Count;
            ReshippedOrders = SQL.Execute("uspGetReshippedOrderCount",
                new Param("BeginDate", Start.Date), new Param("EndDate", End.Date)).Rows.Count;

            DataTable fields = SQL.Execute("uspGetDashboard", new Param("BeginDate", Start.Date), new Param("EndDate", End.Date));
            Fields = fields.Rows[0].Table.Columns
              .Cast<DataColumn>()
              .ToDictionary(c => c.ColumnName, c => fields.Rows[0][c]);

            DataTable best = SQL.Execute("uspGetBestSellers", new Param("BeginDate", Start.Date), new Param("EndDate", End.Date));
            foreach (DataRow dr in best.Rows) BestSellers.Add(new Book((int)dr["BookID"]), (int)dr["Sales"]);

            DataTable worst = SQL.Execute("uspGetWorstSellers", new Param("BeginDate", Start.Date), new Param("EndDate", End.Date));
            foreach (DataRow dr in worst.Rows) WorstSellers.Add(new Book((int)dr["BookID"]), (int)dr["Sales"]);
        }
    }
}