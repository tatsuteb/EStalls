using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Purchase
{
    public class PurchasePageModel : PageModel
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly ICartService _cartService;

        public PurchasePageModel(
            UserManager<AppUser> userManager,
            ICartService cartService)
        {
            _userManager = userManager;
            _cartService = cartService;
        }

        public IEnumerable<CartItemViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; } = 0;

        public class CartItemViewModel
        {
            public Guid ItemId { get; set; }
            public Guid SellerId { get; set; }
            public string ThumbnailPath { get; set; }
            public string Title { get; set; }
            public string SellerName { get; set; }
            public decimal Price { get; set; }
        }

        protected Guid GetCartId()
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
