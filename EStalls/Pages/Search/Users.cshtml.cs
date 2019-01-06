using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Search
{
    public class UsersModel : SearchPageModel
    {
        private readonly IAppUserService _appUserService;

        public UsersModel(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        public IEnumerable<AppUser> AppUsers { get; set; }

        public void OnGet()
        {
            var appUsers = _appUserService.GetAll();

            if (string.IsNullOrWhiteSpace(Keyword))
            {
                AppUsers = appUsers.ToArray();
                return;
            }

            AppUsers = appUsers.Where(x => x.DisplayName.Contains(Keyword))
                .ToArray();
        }
    }
}