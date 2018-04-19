using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages.Admin
{
    public class DashboardController : Controller
    {
        [Route("Dashboard/LoadSalesByCategory")]
        public IActionResult LoadSalesByCategory()
        {
            Dictionary<string, string> fields = AJAX.GetFields(Request);
            DataTable result = SQL.Execute("uspGetSalesByCategory",
                new Param("BeginDate", DateTime.Parse(fields["start"]).Date),
                new Param("EndDate", DateTime.Parse(fields["end"]).Date));

            List<object[]> columns = new List<object[]>();
            int index = 0;
            foreach (DataRow dr in result.Rows)
            {
                columns.Add(new object[] { dr["CategoryName"], (int)dr["Quantity"] });
            }
            return new JsonResult(columns);
        }

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