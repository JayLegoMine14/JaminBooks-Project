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

        public void OnGet()
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null || !CurrentUser.IsAdmin)
            {
                Response.Redirect("/");
            }

            Start = DateTime.Now.AddDays(-7);
            End = DateTime.Now;

            DataTable fields = SQL.Execute("uspGetDashboard", new Param("BeginDate", Start.Date), new Param("EndDate", End.AddDays(1).Date));
            Fields = fields.Rows[0].Table.Columns
              .Cast<DataColumn>()
              .ToDictionary(c => c.ColumnName, c => fields.Rows[0][c]);
        }
    }
}