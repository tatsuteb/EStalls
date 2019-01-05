using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using EStalls.ValidationAttributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class EditModel : PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public EditModel(
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

        [BindProperty(SupportsGet = true)]
        public Guid ItemId { get; set; }

        [BindProperty]
        public InputItemModel InputItem { get; set; }

        [BindProperty]
        public InputItemDlInfoModel InputItemDlInfo { get; set; }

        public string ReturnUrl { get; set; }

        public class InputItemModel
        {
            [StringLength(100, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "タイトル")]
            public string Title { get; set; }

            [StringLength(500, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "説明")]
            public string Description { get; set; }

            [MinValue(500, ErrorMessage = "{0}は{1}円以上です")]
            [Display(Name = "価格（円）")]
            public int Price { get; set; }

            [Display(Name = "プレビュー用ファイル")]
            public List<IFormFile> PreviewFiles { get; set; }

            [Display(Name = "サムネール用ファイル")]
            public IFormFile ThumbnailFile { get; set; }
        }

        public class InputItemDlInfoModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "{0}は{1}文字以下です")]
            [Display(Name = "バージョン")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "販売データファイル")]
            public List<IFormFile> DlFiles { get; set; }
        }

        private class CheckAccessResult
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


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await CheckAccessAsync();

            if (!result.IsValid)
            {
                return result.ReturnPage;
            }

            var item = result.ValidItem;
            
            InputItem = new InputItemModel()
            {
                Title = item?.Title,
                Description = item?.Description,
                Price = decimal.ToInt32(item?.Price ?? 0)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditItemAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var result = await CheckAccessAsync();
            if (!result.IsValid)
            {
                return result.ReturnPage;
            }
            // var item = await _itemService.GetItemAsync(ItemId);
            // var userId = _userManager.GetUserId(User);
            var item = result.ValidItem;
            var userId = result.ValidUid;

            // ダウンロード情報追加用のキーはモデルチェックから外す
            ModelState.Remove(nameof(InputItemDlInfo.DlFiles));
            ModelState.Remove(nameof(InputItemDlInfo.Version));

            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            try
            {
                // 今のプレビューファイルとサムネールファイルの名前を取得しておく
                var previewFileNames = item.PreviewFileNames.Split(",");
                var thumbnailFileName= item.ThumbnailFileName;

                // 保存用のファイル名を作成
                var newPreviewFileNames = FileUtil.GetHtmlEncodedFileNames(InputItem.PreviewFiles?.ToArray());
                var newThumbFileName = FileUtil.GetHtmlEncodedFileName(InputItem.ThumbnailFile);

                #region DBへ作品情報を保存

                // 作品の基本情報
                var itemToUpdate = new Item()
                {
                    Id = ItemId,
                    Uid = userId,
                    Title = InputItem.Title,
                    Description = InputItem.Description,
                    Price = InputItem.Price,
                    PreviewFileNames = string.Join(",", newPreviewFileNames),
                    ThumbnailFileName = newThumbFileName
                };
                await _itemService.UpdateItemAsync(itemToUpdate);

                #endregion


                #region ファイルの差し替え

                // 作品のディレクトリパス
                var dirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemPreviewFiles,
                    ItemId.ToString()
                });

                // プレビューファイル
                if (InputItem.PreviewFiles != null &&
                    InputItem.PreviewFiles.Any())
                {
                    // 古いプレビューファイルを削除
                    foreach (var fileName in previewFileNames)
                    {
                        FileUtil.DeleteFile(dirPath, fileName);
                    }
                    // 新しいプレビューファイルを保存
                    await FileUtil.SaveFilesAsync(InputItem.PreviewFiles?.ToArray(), dirPath, newPreviewFileNames);
                }

                // サムネイルファイル
                if (InputItem.ThumbnailFile != null)
                {
                    var thumbDirPath = Path.Combine(new[]
                    {
                        dirPath,
                        Constants.DirNames.ItemThumbnailFile
                    });
                    // 古いサムネールを削除
                    FileUtil.DeleteFile(thumbDirPath, thumbnailFileName);
                    // 新しいサムネールを保存
                    await FileUtil.SaveFileAsync(InputItem.ThumbnailFile, thumbDirPath, newThumbFileName);
                }

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
                var newDlFileNames = FileUtil.GetHtmlEncodedFileNames(InputItemDlInfo.DlFiles.ToArray());

                #region DBへ作品情報を保存

                // ダウンロード情報
                var newItemDlInfo = new ItemDlInfo()
                {
                    ItemId = ItemId,
                    Version = InputItemDlInfo.Version,
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
                await FileUtil.SaveFilesAsync(InputItemDlInfo.DlFiles.ToArray(), dlDirPath, newDlFileNames);

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

        private async Task<CheckAccessResult> CheckAccessAsync()
        {
            // 作品存在チェック
            var item = await _itemService.GetItemAsync(ItemId);
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