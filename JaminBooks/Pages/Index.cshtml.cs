using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            DataTable dt = SQL.Execute("uspGetUserByID", new Param("UserID", 1));
        }
    }
}
