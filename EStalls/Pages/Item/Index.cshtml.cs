using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Item
{
    public class IndexModel : PageModel
    {
        private readonly IItemService _itemService;

        public IndexModel(
            IItemService itemService)
        {
            _itemService = itemService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid ItemId { get; set; }

        public Data.Models.Item Item { get; set; }

        public async Task OnGetAsync()
        {
            Item = await _itemService.GetAsync(ItemId);
        }
    }
}