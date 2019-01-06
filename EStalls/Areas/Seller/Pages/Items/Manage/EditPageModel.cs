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
    public class EditPageModel : PageModel
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly IItemService _itemService;

        public EditPageModel(
            UserManager<AppUser> userManager,
            IItemService itemService)
        {
            _userManager = userManager;
            _itemService = itemService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid ItemId { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        protected class CheckAccessResult
        {
            public bool IsValid { get; }
            public IActionResult ReturnPage { get; }
            public Item ValidItem { get; }
            public string ValidUid { get; }

            public CheckAccessResult(bool isValid, IActionResult returnPage, Item validItem = null, string validUid = null)
            {
                IsValid = isValid;
                ReturnPage = returnPage;
                ValidItem = validItem;
                ValidUid = validUid;
            }
        }

        protected async Task<CheckAccessResult> CheckAccessAsync()
        {
            // 作品存在チェック
            var item = await _itemService.GetAsync(ItemId);
            if (item == null)
            {
                StatusMessage = "作品が見つかりません";
                // 作品管理ページへ移動
                return new CheckAccessResult(false, RedirectToPage("Index", StatusMessage));
            }

            // ユーザー存在チェック
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                StatusMessage = "ユーザーが見つかりません";
                // トップページへ移動
                return new CheckAccessResult(false, RedirectToPage("/Index", StatusMessage));
            }

            // 作品所有者チェック
            if (item.Uid != userId)
            {
                StatusMessage = "自分の投稿作品以外は編集できません";
                // 作品管理ページへ移動
                return new CheckAccessResult(false, RedirectToPage("Index", StatusMessage));
            }

            return new CheckAccessResult(true, Page(), item, userId);
        }
    }
}
