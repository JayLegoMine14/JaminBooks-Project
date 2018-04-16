using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public User CurrentUser;
        public DateTime Start;
        public DateTime End;
        public Dictionary<string, object> Fields;
        public Dictionary<Book, int> BestSellers = new Dictionary<Book, int>();
        public Dictionary<Book, int> WorstSellers = new Dictionary<Book, int>();

        public void OnPost()
        {
            Start = DateTime.Parse(Request.Form["start"]);
            End = DateTime.Parse(Request.Form["end"]);
            RenderPage(Start, End);
        }

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

        public void RenderPage(DateTime Start, DateTime end)
        {
            DataTable fields = SQL.Execute("uspGetDashboard", new Param("BeginDate", Start.Date), new Param("EndDate", End.AddDays(1).Date));
            Fields = fields.Rows[0].Table.Columns
              .Cast<DataColumn>()
              .ToDictionary(c => c.ColumnName, c => fields.Rows[0][c]);

            DataTable best = SQL.Execute("uspGetBestSellers", new Param("BeginDate", Start.Date), new Param("EndDate", End.AddDays(1).Date));
            foreach (DataRow dr in best.Rows) BestSellers.Add(new Book((int)dr["BookID"]), (int)dr["Sales"]);

            DataTable worst = SQL.Execute("uspGetWorstSellers", new Param("BeginDate", Start.Date), new Param("EndDate", End.AddDays(1).Date));
            foreach (DataRow dr in worst.Rows) WorstSellers.Add(new Book((int)dr["BookID"]), (int)dr["Sales"]);
        }
    }
}