using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.User
{
    public class PurchaseModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IItemService _itemService;
        private readonly IItemDlInfoService _itemDlInfoService;

        public PurchaseModel(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IItemService itemService,
            IItemDlInfoService itemDlInfoService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _itemService = itemService;
            _itemDlInfoService = itemDlInfoService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Uid { get; set; }

        public IEnumerable<PurchaseItemViewModel> Items { get; set; }

        // NOTE: CartItemViewModel と同じなので共通化した方が良いかも
        public class PurchaseItemViewModel
        {
            public Guid ItemId { get; set; }
            public Guid SellerId { get; set; }
            public string ThumbnailPath { get; set; }
            public string Title { get; set; }
            public string SellerName { get; set; }
            public decimal Price { get; set; }
            public IEnumerable<ItemDlInfo> DlInfos { get; set; }
        }

        public void OnGet()
        {
            // ユーザーに紐づくオーダーIDを取得
            var orderIds = _orderService.GetByUid(Uid)
                .Select(x => x.Id);
            // オーダーの注文詳細を取得
            var orderItems = _orderItemService.GetAll()
                .Where(x => orderIds.Contains(x.OrderId))
                .ToArray();
            // 注文詳細から作品のIDを取得
            var orderItemIds = orderItems
                .Select(x => x.ItemId)
                .ToArray();

            Items = _itemService.GetAll()
                .Where(x => orderItemIds.Contains(x.Id))
                .Select(x =>
                {
                    // 購入作品の注文詳細を取得
                    var orderItem = orderItems.FirstOrDefault(y => y.ItemId == x.Id);

                    // タイトル、販売者名、価格は購入時の情報を表示する
                    return new PurchaseItemViewModel()
                    {
                        ItemId = x.Id,
                        SellerId = new Guid(x.Uid),
                        ThumbnailPath =
                            $"/{Constants.DirNames.ItemPreviewFiles}/{x.Id}/{Constants.DirNames.ItemThumbnailFile}/{x.ThumbnailFileName}",
                        Title = orderItem?.Title,
                        SellerName = orderItem?.SellerDisplayName,
                        Price = orderItem?.Price ?? 0,
                        DlInfos = _itemDlInfoService.GetItemDlInfosByItemId(x.Id)
                    };
                });
        }
    }
}