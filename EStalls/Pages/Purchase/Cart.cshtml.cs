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
    public class CartModel : PurchasePageModel
    {
        private readonly IAppUserService _appUserService;
        private readonly ICartItemService _cartItemService;
        private readonly IItemService _itemService;

        public CartModel(
            UserManager<AppUser> userManager,
            IAppUserService appUserService,
            ICartService cartService,
            ICartItemService cartItemService,
            IItemService itemService) : base(userManager, cartService)
        {
            _appUserService = appUserService;
            _cartItemService = cartItemService;
            _itemService = itemService;
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

            TotalAmount = Items.Sum(x => x.Price);
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(Guid itemId)
        {
            var cartId = GetCartId();
            await _cartItemService.DeleteAsync(cartId, itemId);

            return RedirectToPage();
        }
    }
}