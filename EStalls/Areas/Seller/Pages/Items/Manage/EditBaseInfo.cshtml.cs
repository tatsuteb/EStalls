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
    public class EditBaseInfoModel : EditPageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public EditBaseInfoModel(
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

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            var result = await CheckAccessAsync();

            if (!result.IsValid)
            {
                return result.ReturnPage;
            }

            var item = result.ValidItem;
            
            Input = new InputModel()
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

            var item = result.ValidItem;
            var userId = result.ValidUid;

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
                var newPreviewFileNames = FileUtil.GetHtmlEncodedFileNames(Input.PreviewFiles?.ToArray());
                var newThumbFileName = FileUtil.GetHtmlEncodedFileName(Input.ThumbnailFile);

                #region DBへ作品情報を保存

                // 作品の基本情報
                var itemToUpdate = new Item()
                {
                    Id = ItemId,
                    Uid = userId,
                    Title = Input.Title,
                    Description = Input.Description,
                    Price = Input.Price,
                    PreviewFileNames = string.Join(",", newPreviewFileNames),
                    ThumbnailFileName = newThumbFileName
                };
                await _itemService.UpdateAsync(itemToUpdate);

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
                if (Input.PreviewFiles != null &&
                    Input.PreviewFiles.Any())
                {
                    // 古いプレビューファイルを削除
                    foreach (var fileName in previewFileNames)
                    {
                        FileUtil.DeleteFile(dirPath, fileName);
                    }
                    // 新しいプレビューファイルを保存
                    await FileUtil.SaveFilesAsync(Input.PreviewFiles?.ToArray(), dirPath, newPreviewFileNames);
                }

                // サムネイルファイル
                if (Input.ThumbnailFile != null)
                {
                    var thumbDirPath = Path.Combine(new[]
                    {
                        dirPath,
                        Constants.DirNames.ItemThumbnailFile
                    });
                    // 古いサムネールを削除
                    FileUtil.DeleteFile(thumbDirPath, thumbnailFileName);
                    // 新しいサムネールを保存
                    await FileUtil.SaveFileAsync(Input.ThumbnailFile, thumbDirPath, newThumbFileName);
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
    }
}
