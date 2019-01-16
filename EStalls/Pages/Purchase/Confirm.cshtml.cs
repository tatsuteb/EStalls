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
    
    // TODO: カート一覧を出す仕組みはカートページと同じなので共通化する

    public class ConfirmModel : PurchasePageModel
    {
        private readonly IAppUserService _appUserService;
        private readonly ICartItemService _cartItemService;
        private readonly IItemService _itemService;

        public ConfirmModel(
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

        public IActionResult OnGet()
        {
            // TODO: Redis等から一時保存したトークンを読みだす
            var ccToken = HttpContext.Session.Get<string>(Constants.SessionKeys.CcToken);
            HttpContext.Session.Remove(Constants.SessionKeys.CcToken);
            if (ccToken == null)
                return RedirectToPage("/Purchase/Payment",　"ErrorReturn", new
                {
                    statusMessage = "Error: カード情報をご確認ください。"
                });

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


            /*
             * TODO: このタイミングで販売者が価格を変えると、表示価格と販売価格にずれが生じるので、
             * ロックするか、Redis等にこの時点での販売価格を時間制限付きで記録して決済ほうが安全
             */


            return Page();
        }

        public IActionResult OnPostCheckout()
        {
            var cartId = GetCartId();

            var cartItemIds = _cartItemService.GetAll()
                .Where(x => x.CartId == cartId)
                .Select(x => x.ItemId);

            return RedirectToPage("/Purchase/Complete");
        }
    }
}