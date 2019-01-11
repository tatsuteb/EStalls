using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Item
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;


        public IndexModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IItemService itemService,
            ICartService cartService,
            ICartItemService cartItemService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _itemService = itemService;
            _cartService = cartService;
            _cartItemService = cartItemService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid ItemId { get; set; }

        public Data.Models.Item Item { get; set; }

        public async Task OnGetAsync()
        {
            Item = await _itemService.GetAsync(ItemId);
        }

        public async Task<IActionResult> OnPostAddCartAsync()
        {
            // セッションからカートIDを取得
            var cartId = HttpContext.Session.Get<Guid>("_CartId");

            if (cartId == Guid.Empty)
            {
                cartId = Guid.NewGuid();
                HttpContext.Session.Set(Constants.SessionKeys.CartId, cartId);
            }

            var hasItem = _cartItemService.GetAll()
                .Where(x => x.CartId == cartId)
                .Where(x => x.ItemId == ItemId)
                .Select(x => x.ItemId)
                .Contains(ItemId);

            if (!hasItem)
            {
                await _cartItemService.AddAsync(new CartItem()
                {
                    CartId = cartId,
                    ItemId = ItemId
                });
            }


            return RedirectToPage();
        }
    }
}