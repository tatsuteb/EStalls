using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IItemService _itemService;

        public IndexModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        public Item[] RegisteredItems { get; set; }

        public async Task OnGetAsync()
        {
            RegisteredItems = await _itemService.GetRegisteredItemsAsync();
        }
    }
}