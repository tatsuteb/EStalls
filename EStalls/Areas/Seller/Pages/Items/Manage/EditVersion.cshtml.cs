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
            [StringLength(50, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "�o�[�W����")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "�̔��f�[�^�t�@�C��")]
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
                // �ۑ��p�̃t�@�C�������쐬
                var newDlFileNames = FileUtil.GetHtmlEncodedFileNames(Input.DlFiles.ToArray());

                #region DB�֍�i����ۑ�

                // �_�E�����[�h���
                var newItemDlInfo = new ItemDlInfo()
                {
                    ItemId = ItemId,
                    Version = Input.Version,
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
                await FileUtil.SaveFilesAsync(Input.DlFiles.ToArray(), dlDirPath, newDlFileNames);

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
    }
}