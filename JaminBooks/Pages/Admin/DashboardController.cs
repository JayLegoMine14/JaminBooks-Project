using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Pages.Admin
{
    /// <summary>
    /// Handles AJAX requests for the dashboard page.
    /// </summary>
    public class DashboardController : Controller
    {
        /// <summary>
        /// Load the sales by category data
        /// </summary>
        /// <returns>A list of object arrays with the name of the category at index 0 and the number of sales at index 1</returns>
        [Route("Dashboard/LoadSalesByCategory")]
        public IActionResult LoadSalesByCategory()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            DataTable result = SQL.Execute("uspGetSalesByCategory",
                new Param("BeginDate", DateTime.Parse(fields["start"]).Date),
                new Param("EndDate", DateTime.Parse(fields["end"]).Date));

            List<object[]> columns = new List<object[]>();
            foreach (DataRow dr in result.Rows)
            {
                columns.Add(new object[] { dr["CategoryName"], (int)dr["Quantity"] });
            }
            return new JsonResult(columns);
        }

        /// <summary>
        /// Load the amount of revenue per pay over a given period.
        /// </summary>
        /// <returns>A list of object arrays with the date at index 0, the revenue at index 1, and the gross profit at index 2</returns>
        [Route("Dashboard/LoadDailyRevenue")]
        public IActionResult LoadDailyRevenue()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            DateTime Start = DateTime.Parse(fields["start"]).Date;
            DateTime End = DateTime.Parse(fields["end"]).Date;
            DataTable result = SQL.Execute("uspGetRevenue",
                new Param("BeginDate", Start.Date),
                new Param("EndDate", End.AddDays(1).Date));

            List<object[]> columns = new List<object[]>();
            int row = 0;
            for (DateTime d = Start; d <= End.Date; d = d.AddDays(1))
            {
                if (result.Rows.Count > row && ((DateTime)result.Rows[row]["Date"]).Date == d.Date)
                {
                    columns.Add(new object[] { d.ToString("yyyy-M-d"), result.Rows[row]["Revenue"], result.Rows[row]["GrossProfit"] });
                    row++;
                }
                else
                    columns.Add(new object[] { d.ToString("yyyy-M-d"), 0, 0 });
            }
            return new JsonResult(columns);
        }

        /// <summary>
        /// Load the total revenue on each day over a given period.
        /// </summary>
        /// <returns>A list of object arrays with the date at index 0, the revenue at index 1, and the gross profit at index 2</returns>
        [Route("Dashboard/LoadRevenue")]
        public IActionResult LoadRevenue()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            DateTime Start = DateTime.Parse(fields["start"]).Date;
            DateTime End = DateTime.Parse(fields["end"]).Date;
            DataTable result = SQL.Execute("uspGetRevenue",
                new Param("BeginDate", Start.Date),
                new Param("EndDate", End.AddDays(1).Date));

            List<object[]> columns = new List<object[]>();
            if (result.Rows.Count > 0)
            {
                int row = 0;
                decimal totalRevenue = 0;
                decimal totalGrossProfit = 0;
                for (DateTime d = Start; d <= End.Date; d = d.AddDays(1))
                {
                    if (result.Rows.Count > row && ((DateTime)result.Rows[row]["Date"]).Date == d.Date)
                    {
                        totalRevenue += (decimal)result.Rows[row]["Revenue"];
                        totalGrossProfit += (decimal)result.Rows[row]["GrossProfit"];
                        row++;
                    }

                    columns.Add(new object[] { d.ToString("yyyy-M-d"), totalRevenue, totalGrossProfit });
                }
            }
            return new JsonResult(columns);
        }

        /// <summary>
        /// Load the total number of new users on each day over a given period.
        /// </summary>
        /// <returns>A list of object arrays with the date at index 0 and the number of users at index 1</returns>
        [Route("Dashboard/LoadNewUsers")]
        public IActionResult LoadNewUsers()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            DateTime Start = DateTime.Parse(fields["start"]).Date;
            DateTime End = DateTime.Parse(fields["end"]).Date;
            DataTable result = SQL.Execute("uspGetNewUsers",
                new Param("BeginDate", Start.Date),
                new Param("EndDate", End.AddDays(1).Date));

            List<object[]> columns = new List<object[]>();
            if (result.Rows.Count > 0)
            {
                int row = 0;
                int totalUsers = 0;
                for (DateTime d = Start; d <= End.Date; d = d.AddDays(1))
                {
                    if (result.Rows.Count > row && ((DateTime)result.Rows[row]["Date"]).Date == d.Date)
                    {
                        totalUsers += (int)result.Rows[row]["Quantity"];
                        row++;
                    }

                    columns.Add(new object[] { d.ToString("yyyy-M-d"), totalUsers });
                }
            }
            return new JsonResult(columns);
        }
    }
}