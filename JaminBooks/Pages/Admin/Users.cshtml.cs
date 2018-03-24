using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public IList<User> Users { get; private set; }

        public void OnGet()
        {
            Users = new List<User>();
            Users.Add(new User(1));
        }

        public void OnPostDelete(int id)
        {
            var user = new User(id);
            user.Delete();
        }
    }
}
