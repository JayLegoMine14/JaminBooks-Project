using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Pages.Admin
{
    public class SearchController : Controller
    {
        [Route("Search/LoadOrders")]
        public IActionResult LoadOrders()
        {
            User CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser.IsAdmin)
            {
                Dictionary<string, string> fields = AJAX.GetFields(Request);
                int callID = Convert.ToInt32(fields["CallID"]);
                int index = Convert.ToInt32(fields["Index"]);
                int count = Convert.ToInt32(fields["Count"]);
                bool advanced = Convert.ToBoolean(fields["DoAdvancedSearch"]);

                string search = fields["Search"];
                int sortType = Convert.ToInt32(fields["SortColumn"]);
                int sortOrder = Convert.ToInt32(fields["SortOrder"]);

                JArray aSearch = JArray.Parse(fields["AdvancedSearch"]);

                if (search.StartsWith("$"))
                    search = search.Replace("$", "");
                else
                    search = "%" + search.Trim().Replace(" ", "%") + "%";

                List<Order> itemsToSort = new List<Order>();
                if (!advanced)
                {
                    itemsToSort = Order.GetOrders(SQL.Execute("uspSearchOrderByAll", new Param("Search", search)));
                }
                else
                {
                    List<Order> all = Order.GetAll();
                    List<Order> matches = new List<Order>();
                    bool? groupLink = null;
                    foreach (JArray group in aSearch.Children<JArray>())
                    {

                        List<Order> holder = new List<Order>();
                        List<Order> some = new List<Order>();
                        bool? itemLink = null;
                        foreach (JObject item in group.Children<JObject>())
                        {
                            string column = item["Column"].ToString();
                            string comp = item["Comparison"].ToString();
                            string val = item["Value"].ToString();
                            bool? or = (bool?)item["Link"];

                            List<Order> searchItems = (!itemLink.HasValue || itemLink.Value) ? all : some.Count > 0 ? some : matches;

                            switch (column)
                            {
                                case "OrderID":
                                    int value = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o => 
                                            o.OrderID.ToString() == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o => 
                                            !Int32.TryParse(val, out value) ? false : o.OrderID > value
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.OrderID < value
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.OrderID >= value
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.OrderID <= value
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.OrderID.ToString() != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.OrderID.ToString().Contains(val) || val.Contains(o.OrderID.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "OrderDate":
                                    DateTime date = new DateTime();
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date == date.Date
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date > date.Date
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date < date.Date
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date >= date.Date
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date <= date.Date
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.OrderDate.Date != date.Date
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.OrderDate.Date.ToString().Contains(val) || val.Contains(o.OrderDate.Date.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "FirstName":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Card.User.FirstName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName.CompareTo(val) == 1 || o.Card.User.FirstName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName.CompareTo(val) == -1 || o.Card.User.FirstName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.FirstName.Contains(val) || val.Contains(o.Card.User.FirstName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "LastName":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Card.User.LastName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName.CompareTo(val) == 1 || o.Card.User.LastName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName.CompareTo(val) == -1 || o.Card.User.LastName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Card.User.LastName.Contains(val) || val.Contains(o.Card.User.LastName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Items":
                                    int itemCount = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Books.Count == itemCount
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Books.Count > count
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Books.Count < itemCount
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Int32.TryParse(val, out itemCount) ? false : o.Books.Count >= itemCount
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Books.Count <= itemCount
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Books.Count != itemCount
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Books.Count.ToString().Contains(val) || val.Contains(o.Books.Count.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Item":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10 == val || b.ISBN13 == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == 1 || b.ISBN13.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == -1 || b.ISBN13.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == 1 || b.ISBN10 == val || b.ISBN13.CompareTo(val) == 1 || b.ISBN13 == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == -1 || b.ISBN10 == val || b.ISBN13.CompareTo(val) == -1 || b.ISBN13 == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10 != val || b.ISBN13 != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Books.Keys.Any(b => b.ISBN10.Contains(val) || val.Contains(b.ISBN10) || b.ISBN13.Contains(val) || val.Contains(b.ISBN13))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Total":
                                    decimal total = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total == total
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total > total
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total < total
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total >= total
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total <= total
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out total) ? false : o.Total != total
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Total.ToString().Contains(val) || val.Contains(o.Total.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "PercentDiscount":
                                    int discount = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.PercentDiscount.ToString() == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out discount) ? false : o.PercentDiscount > discount
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out discount) ? false : o.PercentDiscount < discount
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out discount) ? false : o.PercentDiscount >= discount
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out discount) ? false : o.PercentDiscount <= discount
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.PercentDiscount.ToString() != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.PercentDiscount.ToString().Contains(val) || val.Contains(o.PercentDiscount.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsFulfilled":
                                    bool fullfilled = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : o.IsFulfilled == fullfilled
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : o.IsFulfilled
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : !o.IsFulfilled
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out fullfilled) ? false : o.IsFulfilled
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : !o.IsFulfilled
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : o.IsFulfilled != fullfilled
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out fullfilled) ? false : o.IsFulfilled == fullfilled
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsRefunded":
                                    bool refunded = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : o.IsRefunded == refunded
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : o.IsRefunded
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : !o.IsRefunded
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out refunded) ? false : o.IsRefunded
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : !o.IsRefunded
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : o.IsRefunded != refunded
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out refunded) ? false : o.IsRefunded == refunded
                                            ).ToList();
                                            break;
                                    }
                                    break;
                            }

                            if (!itemLink.HasValue)
                                some = holder.ToList();
                            else if (itemLink.Value)
                                some.AddRange(holder.Where(i => !some.Contains(i)));
                            else if (!itemLink.Value)
                                some = holder;

                            itemLink = or;
                        }

                        if (!groupLink.HasValue)
                            matches = some.ToList();
                        else if (groupLink.Value)
                            matches.AddRange(some.Where(i => !matches.Contains(i)));
                        else if (!groupLink.Value)
                            matches = some;

                        groupLink = itemLink;
                    }

                    itemsToSort = matches;
                }

                switch (sortType)
                {
                    case 1:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.OrderID.CompareTo(b.OrderID));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.OrderID.CompareTo(a.OrderID));
                                break;
                        }
                        break;
                    case 2:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.OrderDate.CompareTo(b.OrderDate));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.OrderDate.CompareTo(a.OrderDate));
                                break;
                        }
                        break;
                    case 3:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Card.User.LastFirstName.CompareTo(b.Card.User.LastFirstName));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Card.User.LastFirstName.CompareTo(a.Card.User.LastFirstName));
                                break;
                        }
                        break;
                    case 4:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Books.Count.CompareTo(b.Books.Count));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Books.Count.CompareTo(a.Books.Count));
                                break;
                        }
                        break;
                    case 5:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Total.CompareTo(b.Total));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Total.CompareTo(a.Total));
                                break;
                        }
                        break;
                }

                var results = itemsToSort.Count;
                count = itemsToSort.Count - index < count ? itemsToSort.Count - index : count;
                itemsToSort = itemsToSort.GetRange(index, count);

                List<object> items = new List<object>();

                foreach (Order order in itemsToSort)
                {
                    items.Add(new
                    {
                        ID = order.OrderID,
                        OrderDate = order.OrderDate.ToString("MM/dd/yyyy"),
                        Customer = order.Card.User.LastFirstName,
                        Items = order.Books.Count,
                        Total = "$" + order.Total.ToString("0.00")
                    });
                }

                return new JsonResult(new object[] { callID, count, results, items });
            }
            return new JsonResult("");
        }

        [Route("Search/LoadAccounts")]
        public IActionResult LoadAccounts()
        {
            User CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser.IsAdmin)
            {
                Dictionary<string, string> fields = AJAX.GetFields(Request);
                int callID = Convert.ToInt32(fields["CallID"]);
                int index = Convert.ToInt32(fields["Index"]);
                int count = Convert.ToInt32(fields["Count"]);
                bool advanced = Convert.ToBoolean(fields["DoAdvancedSearch"]);

                string search = fields["Search"];
                int sortType = Convert.ToInt32(fields["SortColumn"]);
                int sortOrder = Convert.ToInt32(fields["SortOrder"]);

                JArray aSearch = JArray.Parse(fields["AdvancedSearch"]);

                if (search.StartsWith("$"))
                    search = search.Replace("$", "");
                else
                    search = "%" + search.Trim().Replace(" ", "%") + "%";

                List<User> itemsToSort = new List<User>();
                if (!advanced)
                {
                    itemsToSort = Model.User.getUsers(SQL.Execute("uspSearchUserByAll", new Param("Search", search)));
                }
                else
                {
                    List<User> all = Model.User.GetUsers();
                    List<User> matches = new List<User>();
                    bool? groupLink = null;
                    foreach (JArray group in aSearch.Children<JArray>())
                    {

                        List<User> holder = new List<User>();
                        List<User> some = new List<User>();
                        bool? itemLink = null;
                        foreach (JObject item in group.Children<JObject>())
                        {
                            string column = item["Column"].ToString();
                            string comp = item["Comparison"].ToString();
                            string val = item["Value"].ToString();
                            bool? or = (bool?)item["Link"];

                            List<User> searchItems = (!itemLink.HasValue || itemLink.Value) ? all : some.Count > 0 ? some : matches;

                            switch (column)
                            {
                                case "UserID":
                                    int value = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.UserID.ToString() == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.UserID > value
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.UserID < value
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.UserID >= value
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.UserID <= value
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.UserID.ToString() != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.UserID.ToString().Contains(val) || val.Contains(o.UserID.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Email":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Email == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Email.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Email.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Email.CompareTo(val) == 1 || o.Email.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Email.CompareTo(val) == -1 || o.Email.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Email != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Email.Contains(val) || val.Contains(o.Email)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "FirstName":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.FirstName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.FirstName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.FirstName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.FirstName.CompareTo(val) == 1 || o.FirstName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.FirstName.CompareTo(val) == -1 || o.FirstName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.FirstName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.FirstName.Contains(val) || val.Contains(o.FirstName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "LastName":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.LastName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.LastName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.LastName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.LastName.CompareTo(val) == 1 || o.LastName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.LastName.CompareTo(val) == -1 || o.LastName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.LastName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.LastName.Contains(val) || val.Contains(o.LastName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Orders":
                                    int itemCount = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count == itemCount
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count > count
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count < itemCount
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count >= itemCount
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count <= itemCount
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out itemCount) ? false : o.Orders.Count != itemCount
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Count.ToString().Contains(val) || val.Contains(o.Orders.Count.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Book":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10 == val || b.ISBN13 == val)
                                            )).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == 1 || b.ISBN13.CompareTo(val) == 1)
                                            )).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == -1 || b.ISBN13.CompareTo(val) == -1)
                                            )).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == 1 || b.ISBN10 == val || b.ISBN13.CompareTo(val) == 1 || b.ISBN13 == val)
                                            )).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10.CompareTo(val) == -1 || b.ISBN10 == val || b.ISBN13.CompareTo(val) == -1 || b.ISBN13 == val)
                                            )).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10 != val || b.ISBN13 != val)
                                            )).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Orders.Any(t => t.Books.Keys.Any(b => b.ISBN10.Contains(val) || val.Contains(b.ISBN10) || b.ISBN13.Contains(val) || val.Contains(b.ISBN13))
                                            )).ToList();
                                            break;
                                    }
                                    break;
                                case "Card":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Cards.Any(b => b.LastFourDigits.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits.CompareTo(val) == 1 || b.LastFourDigits == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits.CompareTo(val) == -1 || b.LastFourDigits == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Cards.Any(b => b.LastFourDigits.Contains(val) || val.Contains(b.LastFourDigits))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Phone":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty) == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty).CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty).CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty).CompareTo(val) == 1 || Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty) == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty).CompareTo(val) == -1 || Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty) == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty) != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Phones.Any(b => Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty).Contains(val) || val.Contains(Regex.Replace(b.Number, "[(,+,),-, ]", string.Empty)))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "City":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Addresses.Any(b => b.City.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City.CompareTo(val) == 1 || b.City == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City.CompareTo(val) == -1 || b.City == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.City.Contains(val) || val.Contains(b.City))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "State":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Addresses.Any(b => b.State.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State.CompareTo(val) == 1 || b.State == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State.CompareTo(val) == -1 || b.State == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.State.Contains(val) || val.Contains(b.State))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Country":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Addresses.Any(b => b.Country.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country.CompareTo(val) == 1 || b.Country == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country.CompareTo(val) == -1 || b.Country == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.Country.Contains(val) || val.Contains(b.Country))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "ZIP":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Addresses.Any(b => b.ZIP.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP.CompareTo(val) == 1 || b.ZIP == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP.CompareTo(val) == -1 || b.ZIP == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Addresses.Any(b => b.ZIP.Contains(val) || val.Contains(b.ZIP))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsDeleted":
                                    bool deleted = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted == deleted
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : !o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : !o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted != deleted
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted == deleted
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsConfirmed":
                                    bool confirmed = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : o.IsConfirmed == confirmed
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : o.IsConfirmed
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : !o.IsConfirmed
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out confirmed) ? false : o.IsConfirmed
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : !o.IsConfirmed
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : o.IsConfirmed != confirmed
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out confirmed) ? false : o.IsConfirmed == confirmed
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsAdmin":
                                    bool admin = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : o.IsAdmin == admin
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : o.IsAdmin
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : !o.IsAdmin
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out admin) ? false : o.IsAdmin
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : !o.IsAdmin
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : o.IsAdmin != admin
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out admin) ? false : o.IsAdmin == admin
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "CreationDate":
                                    DateTime date = new DateTime();
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date == date.Date
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date > date.Date
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date < date.Date
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date >= date.Date
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date <= date.Date
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.CreationDate.Date != date.Date
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.CreationDate.Date.ToString().Contains(val) || val.Contains(o.CreationDate.Date.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                            }

                            if (!itemLink.HasValue)
                                some = holder.ToList();
                            else if (itemLink.Value)
                                some.AddRange(holder.Where(i => !some.Contains(i)));
                            else if (!itemLink.Value)
                                some = holder;

                            itemLink = or;
                        }

                        if (!groupLink.HasValue)
                            matches = some.ToList();
                        else if (groupLink.Value)
                            matches.AddRange(some.Where(i => !matches.Contains(i)));
                        else if (!groupLink.Value)
                            matches = some;

                        groupLink = itemLink;
                    }

                    itemsToSort = matches;
                }

                switch (sortType)
                {
                    case 1:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.UserID.CompareTo(b.UserID));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.UserID.CompareTo(a.UserID));
                                break;
                        }
                        break;
                    case 2:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.HasIcon ? 0 : -1 );
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.HasIcon ? 0 : -1);
                                break;
                        }
                        break;
                    case 3:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Email.CompareTo(b.Email));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Email.CompareTo(a.Email));
                                break;
                        }
                        break;
                    case 4:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.FullName.CompareTo(b.FullName));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.FullName.CompareTo(a.FullName));
                                break;
                        }
                        break;
                    case 5:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.CreationDate.CompareTo(b.CreationDate));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.CreationDate.CompareTo(a.CreationDate));
                                break;
                        }
                        break;
                    case 6:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.IsDeleted.CompareTo(b.IsDeleted));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.IsDeleted.CompareTo(a.IsDeleted));
                                break;
                        }
                        break;
                }

                var results = itemsToSort.Count;
                count = itemsToSort.Count - index < count ? itemsToSort.Count - index : count;
                itemsToSort = itemsToSort.GetRange(index, count);

                List<object> items = new List<object>();

                foreach (User user in itemsToSort)
                {
                    items.Add(new
                    {
                        ID = user.UserID,
                        Icon = "<div class='icon-div'>" 
                        + "<img class='userIcon' src='" + user.LoadImage + "' />" 
                        + "</div>",
                        user.Email,
                        Name = user.FullName,
                        CreationDate = user.CreationDate.ToString("MM/dd/yyyy"),
                        Deleted = user.IsDeleted
                    });
                }

                return new JsonResult(new object[] { callID, count, results, items });
            }
            return new JsonResult("");
        }

        [Route("Search/LoadBooks")]
        public IActionResult LoadBooks()
        {
            User CurrentUser = Authentication.GetCurrentUser(HttpContext);
            if (CurrentUser.IsAdmin)
            {
                Dictionary<string, string> fields = AJAX.GetFields(Request);
                int callID = Convert.ToInt32(fields["CallID"]);
                int index = Convert.ToInt32(fields["Index"]);
                int count = Convert.ToInt32(fields["Count"]);
                bool advanced = Convert.ToBoolean(fields["DoAdvancedSearch"]);

                string search = fields["Search"];
                int sortType = Convert.ToInt32(fields["SortColumn"]);
                int sortOrder = Convert.ToInt32(fields["SortOrder"]);

                JArray aSearch = JArray.Parse(fields["AdvancedSearch"]);

                if (search.StartsWith("$"))
                    search = search.Replace("$", "");
                else
                    search = "%" + search.Trim().Replace(" ", "%") + "%";

                List<Book> itemsToSort = new List<Book>();
                if (!advanced)
                {
                    itemsToSort = Book.GetBooks(SQL.Execute("uspSearchBookByAll", new Param("Search", search)));
                }
                else
                {
                    List<Book> all = Model.Book.GetBooks();
                    List<Book> matches = new List<Book>();
                    bool? groupLink = null;
                    foreach (JArray group in aSearch.Children<JArray>())
                    {

                        List<Book> holder = new List<Book>();
                        List<Book> some = new List<Book>();
                        bool? itemLink = null;
                        foreach (JObject item in group.Children<JObject>())
                        {
                            string column = item["Column"].ToString();
                            string comp = item["Comparison"].ToString();
                            string val = item["Value"].ToString();
                            bool? or = (bool?)item["Link"];

                            List<Book> searchItems = (!itemLink.HasValue || itemLink.Value) ? all : some.Count > 0 ? some : matches;

                            switch (column)
                            {
                                case "BookID":
                                    int value = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.BookID.ToString() == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.BookID > value
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.BookID < value
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.BookID >= value
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out value) ? false : o.BookID <= value
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.BookID.ToString() != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.BookID.ToString().Contains(val) || val.Contains(o.BookID.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Title":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Title == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Title.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Title.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Title.CompareTo(val) == 1 || o.Title.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Title.CompareTo(val) == -1 || o.Title.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Title != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Title.Contains(val) || val.Contains(o.Title)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "ISBN13":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13 == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.ISBN13.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13.CompareTo(val) == 1 || o.ISBN13.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13.CompareTo(val) == -1 || o.ISBN13.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13 != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.ISBN13.Contains(val) || val.Contains(o.ISBN13)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "ISBN10":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10 == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.ISBN10.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10.CompareTo(val) == 1 || o.ISBN10.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10.CompareTo(val) == -1 || o.ISBN10.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10 != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.ISBN10.Contains(val) || val.Contains(o.ISBN10)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Description":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Description == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Description.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Description.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Description.CompareTo(val) == 1 || o.Description.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Description.CompareTo(val) == -1 || o.Description.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Description != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Description.Contains(val) || val.Contains(o.Description)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Price":
                                    decimal price = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price == price
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price > price
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price < price
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price >= price
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price <= price
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out price) ? false : o.Price != price
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Price.ToString().Contains(val) || val.Contains(o.Price.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Cost":
                                    decimal cost = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost == cost
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost > cost
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost < cost
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost >= cost
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost <= cost
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Decimal.TryParse(val, out cost) ? false : o.Cost != cost
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Cost.ToString().Contains(val) || val.Contains(o.Cost.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Quantity":
                                    int quan = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out quan) ? false : o.Quantity == quan
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out quan) ? false : o.Quantity > count
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out quan) ? false : o.Quantity < quan
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Int32.TryParse(val, out quan) ? false : o.Quantity >= quan
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out quan) ? false : o.Quantity <= quan
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out quan) ? false : o.Quantity != quan
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Quantity.ToString().Contains(val) || val.Contains(o.Quantity.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Sales":
                                    int sales = 0;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out sales) ? false : o.Sales == sales
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out sales) ? false : o.Sales > sales
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out sales) ? false : o.Sales < sales
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Int32.TryParse(val, out sales) ? false : o.Sales >= sales
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out sales) ? false : o.Sales <= sales
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Int32.TryParse(val, out sales) ? false : o.Sales != sales
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Sales.ToString().Contains(val) || val.Contains(o.Sales.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Author":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Authors.Any(b => b.FullName.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName.CompareTo(val) == 1 || b.FullName == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName.CompareTo(val) == -1 || b.FullName == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Authors.Any(b => b.FullName.Contains(val) || val.Contains(b.FullName))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "Category":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName == val)
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName.CompareTo(val) == 1)
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Categories.Any(b => b.CategoryName.CompareTo(val) == -1)
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName.CompareTo(val) == 1 || b.CategoryName == val)
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName.CompareTo(val) == -1 || b.CategoryName == val)
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName != val)
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Categories.Any(b => b.CategoryName.Contains(val) || val.Contains(b.CategoryName))
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "PublicationDate":
                                    DateTime date = new DateTime();
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date == date.Date
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date > date.Date
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date < date.Date
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date >= date.Date
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date <= date.Date
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date) ? false : o.PublicationDate.Date != date.Date
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.PublicationDate.Date.ToString().Contains(val) || val.Contains(o.PublicationDate.Date.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "CopyrightDate":
                                    DateTime date2 = new DateTime();
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date == date2.Date
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date > date2.Date
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date < date2.Date
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date >= date2.Date
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date <= date2.Date
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !DateTime.TryParse(val, out date2) ? false : o.CopyrightDate.Date != date2.Date
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.CopyrightDate.Date.ToString().Contains(val) || val.Contains(o.CopyrightDate.Date.ToString())
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "PublisherName":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Publisher.PublisherName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName.CompareTo(val) == 1 || o.Publisher.PublisherName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName.CompareTo(val) == -1 || o.Publisher.PublisherName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.PublisherName.Contains(val) || val.Contains(o.Publisher.PublisherName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "PublisherContact":
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName == val
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName.CompareTo(val) == 1
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                                o.Publisher.FullName.CompareTo(val) == -1
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName.CompareTo(val) == 1 || o.Publisher.FullName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName.CompareTo(val) == -1 || o.Publisher.FullName.CompareTo(val) == 0
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName != val
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            o.Publisher.FullName.Contains(val) || val.Contains(o.Publisher.FullName)
                                            ).ToList();
                                            break;
                                    }
                                    break;
                                case "IsDeleted":
                                    bool deleted = false;
                                    switch (comp)
                                    {
                                        case "eq":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted == deleted
                                            ).ToList();
                                            break;
                                        case "gt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "lt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : !o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "ge":
                                            holder = searchItems.Where(o =>
                                           !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted
                                           ).ToList();
                                            break;
                                        case "le":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : !o.IsDeleted
                                            ).ToList();
                                            break;
                                        case "nt":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted != deleted
                                            ).ToList();
                                            break;
                                        case "lk":
                                            holder = searchItems.Where(o =>
                                            !Boolean.TryParse(val, out deleted) ? false : o.IsDeleted == deleted
                                            ).ToList();
                                            break;
                                    }
                                    break;
                            }

                            if (!itemLink.HasValue)
                                some = holder.ToList();
                            else if (itemLink.Value)
                                some.AddRange(holder.Where(i => !some.Contains(i)));
                            else if (!itemLink.Value)
                                some = holder;

                            itemLink = or;
                        }

                        if (!groupLink.HasValue)
                            matches = some.ToList();
                        else if (groupLink.Value)
                            matches.AddRange(some.Where(i => !matches.Contains(i)));
                        else if (!groupLink.Value)
                            matches = some;

                        groupLink = itemLink;
                    }

                    itemsToSort = matches;
                }

                switch (sortType)
                {
                    case 1:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.BookID.CompareTo(b.BookID));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.BookID.CompareTo(a.BookID));
                                break;
                        }
                        break;
                    case 2:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.HasIcon ? 0 : -1);
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.HasIcon ? 0 : -1);
                                break;
                        }
                        break;
                    case 3:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Title.CompareTo(b.Title));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Title.CompareTo(a.Title));
                                break;
                        }
                        break;
                    case 4:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => string.Join(", ", a.Authors.Select(c => c.FullName)).CompareTo(string.Join(", ", b.Authors.Select(c => c.FullName))));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => string.Join(", ", b.Authors.Select(c => c.FullName)).CompareTo(string.Join(", ", a.Authors.Select(c => c.FullName))));
                                break;
                        }
                        break;
                    case 5:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.ISBN13.CompareTo(b.ISBN13));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.ISBN13.CompareTo(a.ISBN13));
                                break;
                        }
                        break;
                    case 6:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Price.CompareTo(b.Price));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Price.CompareTo(a.Price));
                                break;
                        }
                        break;
                    case 7:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Cost.CompareTo(b.Cost));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Cost.CompareTo(a.Cost));
                                break;
                        }
                        break;
                    case 8:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.Quantity.CompareTo(b.Quantity));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.Quantity.CompareTo(a.Quantity));
                                break;
                        }
                        break;
                    case 9:
                        switch (sortOrder)
                        {
                            case 1:
                                itemsToSort.Sort((a, b) => a.IsDeleted.CompareTo(b.IsDeleted));
                                break;
                            case -1:
                                itemsToSort.Sort((a, b) => b.IsDeleted.CompareTo(a.IsDeleted));
                                break;
                        }
                        break;
                }

                var results = itemsToSort.Count;
                count = itemsToSort.Count - index < count ? itemsToSort.Count - index : count;
                itemsToSort = itemsToSort.GetRange(index, count);

                List<object> items = new List<object>();

                foreach (Book book in itemsToSort)
                {
                    items.Add(new
                    {
                        ID = book.BookID,
                        Image = "<img style='width:auto' height=75 src='" + book.LoadImage + "' />",
                        book.Title,
                        Authors = string.Join(", ", book.Authors.Select(c => c.FullName)),
                        ISBN = book.ISBN13,
                        Price = "$" + book.Price.ToString("0.00"),
                        Cost = "$" + book.Cost.ToString("0.00"),
                        book.Quantity,
                        Deleted = book.IsDeleted
                    });
                }

                return new JsonResult(new object[] { callID, count, results, items });
            }
            return new JsonResult("");
        }
    }
}