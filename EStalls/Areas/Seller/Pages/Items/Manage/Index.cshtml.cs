using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;

        public IndexModel(
            UserManager<AppUser> userManager,
            IItemService itemService)
        {
            _userManager = userManager;
            _itemService = itemService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public Item[] RegisteredItems { get; set; }

        public void OnGet()
        {
            var uid = _userManager.GetUserId(User);
            RegisteredItems = _itemService.GetByUid(uid)
                .ToArray();
        }
    }
}