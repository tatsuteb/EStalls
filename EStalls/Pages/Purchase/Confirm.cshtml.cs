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
    
    // TODO: �J�[�g�ꗗ���o���d�g�݂̓J�[�g�y�[�W�Ɠ����Ȃ̂ŋ��ʉ�����

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
            // TODO: Redis������ꎞ�ۑ������g�[�N����ǂ݂���
            var ccToken = HttpContext.Session.Get<string>(Constants.SessionKeys.CcToken);
            HttpContext.Session.Remove(Constants.SessionKeys.CcToken);
            if (ccToken == null)
                return RedirectToPage("/Purchase/Payment",�@"ErrorReturn", new
                {
                    statusMessage = "Error: �J�[�h�������m�F���������B"
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
             * TODO: ���̃^�C�~���O�Ŕ̔��҂����i��ς���ƁA�\�����i�Ɣ̔����i�ɂ��ꂪ������̂ŁA
             * ���b�N���邩�ARedis���ɂ��̎��_�ł̔̔����i�����Ԑ����t���ŋL�^���Č��ςق������S
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