using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserService _appUserService;

        public ProfileModel(
            UserManager<AppUser> userManager,
            IAppUserService appUserService)
        {
            _userManager = userManager;
            _appUserService = appUserService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Uid { get; set; }

        public string DisplayName { get; set; }
        public bool IsSeller { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _appUserService.GetAsync(Uid.ToString());
            DisplayName = user.DisplayName;
            IsSeller = await _userManager.IsInRoleAsync(user, Constants.RoleTypes.Seller);
        }
    }
}