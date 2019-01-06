using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class EditVersionModel : EditPageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public EditVersionModel(
            IHostingEnvironment environment,
            UserManager<AppUser> userManager,
            IItemService itemService,
            IItemDlInfoService itemDlInfoService,
            ILogger<RegisterModel> logger) : base(userManager, itemService)
        {
            _environment = environment;
            _itemDlInfoService = itemDlInfoService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "バージョン")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "販売データファイル")]
            public List<IFormFile> DlFiles { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await CheckAccessAsync();

            if (!result.IsValid)
            {
                return result.ReturnPage;
            }

            var item = result.ValidItem;

            return Page();
        }

        public async Task<IActionResult> OnPostAddVersionAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var result = await CheckAccessAsync();
            if (!result.IsValid)
            {
                return result.ReturnPage;
            }

            if (!ModelState.IsValid)
            {
                RedirectToPage();
            }

            try
            {
                // 保存用のファイル名を作成
                var newDlFileNames = FileUtil.GetHtmlEncodedFileNames(Input.DlFiles.ToArray());

                #region DBへ作品情報を保存

                // ダウンロード情報
                var newItemDlInfo = new ItemDlInfo()
                {
                    ItemId = ItemId,
                    Version = Input.Version,
                    DlFileNames = string.Join(",", newDlFileNames),
                };
                await _itemDlInfoService.AddItemDlInfoAsync(newItemDlInfo);

                #endregion


                #region ファイルのアップロード

                // ダウンロード用ファイル
                var dlDirPath = Path.Combine(new[]
                {
                    _environment.WebRootPath,
                    Constants.DirNames.ItemDlFiles,
                    newItemDlInfo.Id.ToString()
                });
                await FileUtil.SaveFilesAsync(Input.DlFiles.ToArray(), dlDirPath, newDlFileNames);

                #endregion
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);

                return LocalRedirect(returnUrl);
            }

            StatusMessage = "新しいバージョンが追加されました";
            return RedirectToPage();
        }
    }
}