using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;
using EStalls.Services.Items;
using EStalls.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Seller.Pages.Items
{
    [Area("Seller")]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IItemService _itemService;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<AppUser> userManager,
            IItemService itemService,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _itemService = itemService;
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
            [StringLength(100, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "�^�C�g��")]
            public string Title { get; set; }

            [Required]
            [StringLength(500, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "����")]
            public string Description { get; set; }

            [Required]
            [MinValue(500, ErrorMessage = "{0}��{1}�~�ȏ�ł�")]
            [Display(Name = "���i�i�~�j")]
            public int Price { get; set; }

            [Required]
            [Display(Name = "�v���r���[�p�t�@�C��")]
            public List<IFormFile> PreviewFiles { get; set; }

            [Required]
            [Display(Name = "�T���l�[���p�t�@�C��")]
            public IFormFile ThumbnailFile { get; set; }


            [Required]
            [StringLength(50, ErrorMessage = "{0}��{1}�����ȉ��ł�")]
            [Display(Name = "�o�[�W����")]
            public string Version { get; set; }

            [Required]
            [Display(Name = "�̔��f�[�^�t�@�C��")]
            public List<IFormFile> DlFiles { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                try
                {
                    await _itemService.SaveItemAsync(new InputItemModel()
                    {
                        Title = Input.Title,
                        Description = Input.Description,
                        Price = Input.Price,
                        Version = Input.Version,
                        ThumbnailFile = Input.ThumbnailFile,
                        PreviewFiles = Input.PreviewFiles,
                        DlFiles = Input.DlFiles
                    });
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);

                    return LocalRedirect(returnUrl);
                }
            }

            StatusMessage = "��i�����e����܂���";
            return RedirectToPage();
        }
    }
}