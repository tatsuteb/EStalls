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
    public class EditVersionModel : EditPageModel // PageModel
    {
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly IItemDlInfoService _itemDlInfoService;
        private readonly ILogger<RegisterModel> _logger;

        public EditVersionModel(
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

        // [BindProperty(SupportsGet = true)]
        // public Guid ItemId { get; set; }

        [BindProperty]
        public InputItemDlInfoModel InputItemDlInfo { get; set; }

        public string ReturnUrl { get; set; }

        public class InputItemDlInfoModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "�o�[�W����")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "�̔��f�[�^�t�@�C��")]
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
                // �ۑ��p�̃t�@�C�������쐬
                var newDlFileNames = FileUtil.GetHtmlEncodedFileNames(InputItemDlInfo.DlFiles.ToArray());

                #region DB�֍�i����ۑ�

                // �_�E�����[�h���
                var newItemDlInfo = new ItemDlInfo()
                {
                    ItemId = ItemId,
                    Version = InputItemDlInfo.Version,
                    DlFileNames = string.Join(",", newDlFileNames),
                };
                await _itemDlInfoService.AddItemDlInfoAsync(newItemDlInfo);

                #endregion


                #region �t�@�C���̃A�b�v���[�h

                // �_�E�����[�h�p�t�@�C��
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

            StatusMessage = "�V�����o�[�W�������ǉ�����܂���";
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