using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Purchase
{
    public class PaymentModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
        }

        /// <summary>
        /// 別のページからエラーで帰ってきたとき用
        /// </summary>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public IActionResult OnGetErrorReturn(string statusMessage)
        {
            StatusMessage = statusMessage;

            return RedirectToPage();
        }

        public IActionResult OnPost(string ccToken)
        {
            if (string.IsNullOrWhiteSpace(ccToken))
            {
                StatusMessage = "カード情報が不正です。";
                return RedirectToPage();
            }

            // TODO: 実際はUidをキーに含めて、Redis等に一時保存する
            HttpContext.Session.Set(Constants.SessionKeys.CcToken, ccToken);

            return RedirectToPage("/Purchase/Confirm");
        }

    }
}