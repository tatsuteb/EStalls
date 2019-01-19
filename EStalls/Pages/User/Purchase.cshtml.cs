using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.User
{
    public class PurchaseModel : PageModel
    {
        public IEnumerable<PurchaseItemViewModel> Items { get; set; }

        // NOTE: CartItemViewModel ‚Æ“¯‚¶‚È‚Ì‚Å‹¤’Ê‰»‚µ‚½•û‚ª—Ç‚¢‚©‚à
        public class PurchaseItemViewModel
        {
            public Guid ItemId { get; set; }
            public Guid SellerId { get; set; }
            public string ThumbnailPath { get; set; }
            public string Title { get; set; }
            public string SellerName { get; set; }
            public decimal Price { get; set; }
        }

        public void OnGet()
        {
            Items = new[]
            {
                new PurchaseItemViewModel()
                {
                    ItemId = Guid.NewGuid(),
                    SellerId = Guid.NewGuid(),
                    ThumbnailPath = "",
                    Title = "Dummy Title",
                    SellerName = "Dummy",
                    Price = 0
                }
            };
        }
    }
}