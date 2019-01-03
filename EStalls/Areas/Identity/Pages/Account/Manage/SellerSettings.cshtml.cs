using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Identity.Pages.Account.Manage
{
    public class SellerSettingsModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public SellerSettingsModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public bool IsSeller { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "”Ì”„Ò“o˜^")]
            public bool IsSeller { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IsSeller = await _userManager.IsInRoleAsync(user, Constants.RoleTypes.Seller);

            Input = new InputModel
            {
                IsSeller = IsSeller
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // ”Ì”„“o˜^Ò—pƒ[ƒ‹‚ª‘¶İ‚µ‚È‚¢‚È‚ç€”õ‚·‚é
            if (!await _roleManager.RoleExistsAsync(Constants.RoleTypes.Seller))
            {
                await _roleManager.CreateAsync(new IdentityRole(Constants.RoleTypes.Seller));
            }

            // ”Ì”„Ò“o˜^
            if (Input.IsSeller &&
                !await _userManager.IsInRoleAsync(user, Constants.RoleTypes.Seller))
            {
                await _userManager.AddToRoleAsync(user, Constants.RoleTypes.Seller);
                StatusMessage = "”Ì”„Ò“o˜^‚ªŠ®—¹‚µ‚Ü‚µ‚½";
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, Constants.RoleTypes.Seller);
                StatusMessage = "”Ì”„Ò“o˜^‚ª‰ğœ‚³‚ê‚Ü‚µ‚½";
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToPage();
        }
    }
}