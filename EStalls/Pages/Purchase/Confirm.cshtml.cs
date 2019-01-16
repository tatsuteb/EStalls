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
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;

        public ConfirmModel(
            UserManager<AppUser> userManager,
            IAppUserService appUserService,
            ICartService cartService,
            ICartItemService cartItemService,
            IItemService itemService,
            IOrderService orderService,
            IOrderItemService orderItemService) : base(userManager, cartService)
        {
            _appUserService = appUserService;
            _cartItemService = cartItemService;
            _itemService = itemService;
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public IActionResult OnGet()
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


            /*
             * TODO: ���̃^�C�~���O�Ŕ̔��҂����i��ς���ƁA�\�����i�Ɣ̔����i�ɂ��ꂪ������̂ŁA
             * ���b�N���邩�ARedis���ɂ��̎��_�ł̔̔����i�����Ԑ����t���ŋL�^���Č��ςق������S
             */


            return Page();
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            // TODO: Redis������ꎞ�ۑ������g�[�N����ǂ݂���
            var ccToken = HttpContext.Session.Get<string>(Constants.SessionKeys.CcToken);
            HttpContext.Session.Remove(Constants.SessionKeys.CcToken);
            if (ccToken == null)
                return RedirectToPage("/Purchase/Payment", "ErrorReturn", new
                {
                    statusMessage = "Error: �J�[�h�������m�F���������B"
                });

            // TODO: ���Ϗ���

            #region ���Ϗ���

            var cartId = GetCartId();

            var cartItems = _cartItemService.GetAll()
                .Where(x => x.CartId == cartId);

            var cartItemIds = cartItems.Select(x => x.ItemId);

            var totalAmount = _itemService.GetAll()
                .Where(x => cartItemIds.Contains(x.Id))
                .Sum(x => x.Price);

            #endregion


            #region ���[�U�[�ƍw����i��R�Â���

            var uid = _userManager.GetUserId(User);

            // �I�[�_�[���쐬
            var order = new Order()
            {
                Uid = new Guid(uid),
                TotalAmount = totalAmount,
            };
            await _orderService.AddAsync(order);

            // �w����i�����[�U�[�ƕR�Â���
            var orderItems = _itemService.GetAll()
                .Where(x => cartItemIds.Contains(x.Id))
                .Select(x => new OrderItem()
                {
                    OrderId = order.Id,
                    ItemId = x.Id,
                    Title = x.Title,
                    SellerDisplayName = _appUserService.Get(x.Uid).DisplayName,
                    Price = x.Price,
                });
            await _orderItemService.AddRangeAsync(orderItems);

            #endregion

            #region �J�[�g�����i���폜

            await _cartItemService.DeleteRangeAsync(cartItems);

            #endregion

            return RedirectToPage("/Purchase/Complete");
        }
    }
}