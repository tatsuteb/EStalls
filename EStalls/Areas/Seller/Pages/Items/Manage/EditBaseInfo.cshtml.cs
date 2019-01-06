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
    public class EditBaseInfoModel : EditPageModel //PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public EditBaseInfoModel(
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
            [StringLength(100, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "�^�C�g��")]
            public string Title { get; set; }

            [StringLength(500, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "����")]
            public string Description { get; set; }

            [MinValue(500, ErrorMessage = "{0}��{1}�~�ȏ�ł�")]
            [Display(Name = "���i�i�~�j")]
            public int Price { get; set; }

            [Display(Name = "�v���r���[�p�t�@�C��")]
            public List<IFormFile> PreviewFiles { get; set; }

            [Display(Name = "�T���l�[���p�t�@�C��")]
            public IFormFile ThumbnailFile { get; set; }
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
                // ���̃v���r���[�t�@�C���ƃT���l�[���t�@�C���̖��O���擾���Ă���
                var previewFileNames = item.PreviewFileNames.Split(",");
                var thumbnailFileName= item.ThumbnailFileName;

                // �ۑ��p�̃t�@�C�������쐬
                var newPreviewFileNames = FileUtil.GetHtmlEncodedFileNames(Input.PreviewFiles?.ToArray());
                var newThumbFileName = FileUtil.GetHtmlEncodedFileName(Input.ThumbnailFile);

                #region DB�֍�i����ۑ�

                // ��i�̊�{���
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
                await _itemService.UpdateItemAsync(itemToUpdate);

                #endregion


                #region �t�@�C���̍����ւ�

                // ��i�̃f�B���N�g���p�X
                var dirPath = Path.Combine(new[]
                {
                    this._environment.WebRootPath,
                    Constants.DirNames.ItemPreviewFiles,
                    ItemId.ToString()
                });

                // �v���r���[�t�@�C��
                if (Input.PreviewFiles != null &&
                    Input.PreviewFiles.Any())
                {
                    // �Â��v���r���[�t�@�C�����폜
                    foreach (var fileName in previewFileNames)
                    {
                        FileUtil.DeleteFile(dirPath, fileName);
                    }
                    // �V�����v���r���[�t�@�C����ۑ�
                    await FileUtil.SaveFilesAsync(Input.PreviewFiles?.ToArray(), dirPath, newPreviewFileNames);
                }

                // �T���l�C���t�@�C��
                if (Input.ThumbnailFile != null)
                {
                    var thumbDirPath = Path.Combine(new[]
                    {
                        dirPath,
                        Constants.DirNames.ItemThumbnailFile
                    });
                    // �Â��T���l�[�����폜
                    FileUtil.DeleteFile(thumbDirPath, thumbnailFileName);
                    // �V�����T���l�[����ۑ�
                    await FileUtil.SaveFileAsync(Input.ThumbnailFile, thumbDirPath, newThumbFileName);
                }

                #endregion
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);

                return LocalRedirect(returnUrl);
            }

            StatusMessage = "��i�����e����܂���";
            return RedirectToPage();
        }


        private async Task<CheckAccessResult> CheckAccessAsync()
        {
            // ��i���݃`�F�b�N
            var item = await _itemService.GetItemAsync(ItemId);
            if (item == null)
            {
                StatusMessage = "��i��������܂���";
                // ��i�Ǘ��y�[�W�ֈړ�
                return new CheckAccessResult(false, RedirectToPage("Index", StatusMessage));
            }

            // ���[�U�[���݃`�F�b�N
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                StatusMessage = "���[�U�[��������܂���";
                // �g�b�v�y�[�W�ֈړ�
                return new CheckAccessResult(false, RedirectToPage("/Index", StatusMessage));
            }

            // ��i���L�҃`�F�b�N
            if (item.Uid != userId)
            {
                StatusMessage = "�����̓��e��i�ȊO�͕ҏW�ł��܂���";
                // ��i�Ǘ��y�[�W�ֈړ�
                return new CheckAccessResult(false, RedirectToPage("Index", StatusMessage));
            }

            return new CheckAccessResult(true, Page(), item, userId);
        }
    }
}