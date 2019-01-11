using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EStalls.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<AppUser> signInManager,
            ICartService cartService,
            ICartItemService cartItemService,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _cartService = cartService;
            _cartItemService = cartItemService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _signInManager.UserManager.Users
                        .SingleOrDefaultAsync(x => x.UserName == Input.UserName);
                    
                    await InitializeCart(Guid.Parse(user.Id));

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        /// <summary>
        /// カートIDは常にセッションからとれば良いように、
        /// ログインしたタイミングでユーザーのカート結合して、
        /// 最新のカートIDをセッションに記録しておく
        /// </summary>
        /// <returns></returns>
        private async Task InitializeCart(Guid uid)
        {
            // TODO: 条件分岐がややこしいので要リファクタリング

            // セッションからカートIDを取得
            var sessionCartId = HttpContext.Session.Get<Guid>(Constants.SessionKeys.CartId);

            // DBからカートIDを取得
            var dbCartId = _cartService.GetId(uid);

            // ログインしない状態でカートを使っていない場合
            if (sessionCartId == Guid.Empty)
            {
                if (dbCartId != Guid.Empty)
                {
                    // セッションにカートIDを記録
                    HttpContext.Session.Set(Constants.SessionKeys.CartId, dbCartId);
                }

                return;
            }


            // ログインした状態でカートを使っていない場合
            if (dbCartId == Guid.Empty)
            {
                // ログインしない状態で使っていたカートをログインユーザーと紐づける
                await _cartService.AddAsync(new Cart
                {
                    Id = sessionCartId,
                    Uid = uid
                });

                return;
            }


            // ログインしない状態でカートを使っていて、ログインした状態でもカートを使っていた場合は、
            // ログインしない状態で使っていたカートを、ログインしているときに使っていたカートにまとめる
            if (sessionCartId != dbCartId)
            {
                var dbCartItemIds = _cartItemService.GetAll()
                    .Where(x => x.CartId == dbCartId)
                    .Select(x => x.ItemId)
                    .ToArray();

                var sessionCartItems = _cartItemService.GetAll()
                    .Where(x => x.CartId == sessionCartId)
                    .Where(x => !dbCartItemIds.Contains(x.ItemId))
                    .ToArray();

                foreach (var cartItem in sessionCartItems)
                {
                    cartItem.CartId = dbCartId;
                }

                await _cartItemService.UpdateRangeAsync(sessionCartItems);
            }

            // セッションにカートIDを記録
            HttpContext.Session.Set(Constants.SessionKeys.CartId, dbCartId);
        }
    }
}
