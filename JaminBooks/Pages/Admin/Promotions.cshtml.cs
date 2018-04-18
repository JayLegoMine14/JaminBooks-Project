using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JaminBooks.Pages.Admin
{
    public class PromotionsModel : PageModel
    {

        public List<Promotion> TotalPromotions;
        public List<Promotion> CodePromotions;

        public void OnGet()
        {
            List<Promotion> all = Promotion.GetPromotions();
            TotalPromotions = all.Where(p => p.Total != null).ToList();
            TotalPromotions.Sort((a, b) => (b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now).CompareTo((a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)));
            CodePromotions = all.Where(p => p.Code != null).ToList();
            CodePromotions.Sort((a, b) => (b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now).CompareTo((a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now)));
        }
    }
}