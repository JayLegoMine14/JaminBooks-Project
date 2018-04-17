﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages
{
    public class OrdersModel : PageModel
    {
        public User CurrentUser;
        public User DisplayUser;

        public void OnGet(int? id)
        {
            CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser == null)
            {
                Response.Redirect("Index");
            }
            else
            {
                if (CurrentUser.IsAdmin && id != null)
                    DisplayUser = new User(id.Value);
                else
                    DisplayUser = CurrentUser;
            }

        }
    }
}