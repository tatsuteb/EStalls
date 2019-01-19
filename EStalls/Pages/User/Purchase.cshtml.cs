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

        // NOTE: CartItemViewModel �Ɠ����Ȃ̂ŋ��ʉ����������ǂ�����
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
            // ���[�U�[�ɕR�Â��I�[�_�[ID���擾
            var orderIds = _orderService.GetByUid(Uid)
                .Select(x => x.Id);
            // �I�[�_�[�̒����ڍׂ��擾
            var orderItems = _orderItemService.GetAll()
                .Where(x => orderIds.Contains(x.OrderId))
                .ToArray();
            // �����ڍׂ����i��ID���擾
            var orderItemIds = orderItems
                .Select(x => x.ItemId)
                .ToArray();

            Items = _itemService.GetAll()
                .Where(x => orderItemIds.Contains(x.Id))
                .Select(x =>
                {
                    // �w����i�̒����ڍׂ��擾
                    var orderItem = orderItems.FirstOrDefault(y => y.ItemId == x.Id);

                    // �^�C�g���A�̔��Җ��A���i�͍w�����̏���\������
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