using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Purchase
{
    public class CartModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _appUserService;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        private readonly IItemService _itemService;

        public CartModel(
            UserManager<AppUser> userManager,
            IAppUserService appUserService,
            ICartService cartService,
            ICartItemService cartItemService,
            IItemService itemService)
        {
            _userManager = userManager;
            _appUserService = appUserService;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _itemService = itemService;
        }

        public IEnumerable<CartItemViewModel> Items { get; set; }

        public class CartItemViewModel
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
            var cartId = GetCartId();

            var cartItemIds = _cartItemService.GetAll()
                .Where(x => x.CartId == cartId)
                .Select(x => x.ItemId);

            Items = _itemService.GetAll()
                .Where(x => cartItemIds.Contains(x.Id))
                .Select(x => new CartItemViewModel()
                {
                    ItemId = x.Id,
                    SellerId = new Guid(x.Uid),
                    ThumbnailPath = $"/{Constants.DirNames.ItemPreviewFiles}/{x.Id}/{Constants.DirNames.ItemThumbnailFile}/{x.ThumbnailFileName}",
                    Title = x.Title,
                    Price = x.Price,
                    SellerName = _appUserService.Get(x.Uid).DisplayName
                });
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(Guid itemId)
        {
            var cartId = GetCartId();
            await _cartItemService.Delete(cartId, itemId);

            return RedirectToPage();
        }

        private Guid GetCartId()
        {
            var cartId = HttpContext.Session.Get<Guid>(Constants.SessionKeys.CartId);

            if (cartId != Guid.Empty) return cartId;

            var userId = _userManager.GetUserId(User);

            if (userId == null) return Guid.Empty;
            
            cartId = _cartService.GetId(new Guid(userId));

            return cartId;
        }
    }
}