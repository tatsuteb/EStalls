using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Services;
using EStalls.Utilities;
using EStalls.ValidationAttributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Seller.Pages.Items
{
    public class RegisterModel : PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IHostingEnvironment environment,
            UserManager<AppUser> userManager,
            IItemService itemService,
            IItemDlInfoService itemDlInfoService,
            ILogger<RegisterModel> logger)
        {
            _environment = environment;
            _userManager = userManager;
            _itemService = itemService;
            _itemDlInfoService = itemDlInfoService;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "タイトル")]
            public string Title { get; set; }

            [Required]
            [StringLength(500, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "説明")]
            public string Description { get; set; }

            [Required]
            [MinValue(500, ErrorMessage = "{0}は{1}円以上です")]
            [Display(Name = "価格（円）")]
            public int Price { get; set; }

            [Required]
            [Display(Name = "プレビュー用ファイル")]
            public List<IFormFile> PreviewFiles { get; set; }

            [Required]
            [Display(Name = "サムネール用ファイル")]
            public IFormFile ThumbnailFile { get; set; }


            [Required]
            [StringLength(50, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "バージョン")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "販売データファイル")]
            public List<IFormFile> DlFiles { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                RedirectToPage();
            }

            try
            {
                // 保存用のファイル名を作成
                var previewFileNames = FileUtil.GetHtmlEncodedFileNames(Input.PreviewFiles.ToArray());
                var dlFileNames = FileUtil.GetHtmlEncodedFileNames(Input.DlFiles.ToArray());
                var thumbFileName = FileUtil.GetHtmlEncodedFileName(Input.ThumbnailFile);


                #region DBへ作品情報を保存

                // 作品の基本情報
                var newItem = new Item()
                {
                    Uid = _userManager.GetUserId(User),
                    Title = Input.Title,
                    Description = Input.Description,
                    Price = Input.Price,
                    PreviewFileNames = string.Join(",", previewFileNames),
                    ThumbnailFileName = thumbFileName
                };
                await _itemService.AddItemAsync(newItem);

                // ダウンロード情報
                var newItemDlInfo = new ItemDlInfo()
                {
                    ItemId = newItem.Id,
                    Version = Input.Version,
                    DlFileNames = string.Join(",", dlFileNames),
                };
                await _itemDlInfoService.AddItemDlInfoAsync(newItemDlInfo);

                #endregion


                #region ファイルのアップロード

                // プレビューファイル
                var dirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemPreviewFiles,
                    newItem.Id.ToString()
                });
                await FileUtil.SaveFilesAsync(Input.PreviewFiles.ToArray(), dirPath, previewFileNames);

                // サムネイルファイル
                var thumbDirPath = Path.Combine(new[]
                {
                    dirPath,
                    Constants.DirNames.ItemThumbnailFile
                });
                await FileUtil.SaveFileAsync(Input.ThumbnailFile, thumbDirPath, thumbFileName);

                // ダウンロード用ファイル
                var dlDirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemDlFiles,
                    newItemDlInfo.Id.ToString()
                });
                await FileUtil.SaveFilesAsync(Input.DlFiles.ToArray(), dlDirPath, dlFileNames);

                #endregion
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);

                return LocalRedirect(returnUrl);
            }

            StatusMessage = "作品が投稿されました";
            return RedirectToPage();
        }
    }
}