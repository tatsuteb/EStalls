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
        /// �ʂ̃y�[�W����G���[�ŋA���Ă����Ƃ��p
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
                StatusMessage = "�J�[�h��񂪕s���ł��B";
                return RedirectToPage();
            }

            // TODO: ���ۂ�Uid���L�[�Ɋ܂߂āARedis���Ɉꎞ�ۑ�����
            HttpContext.Session.Set(Constants.SessionKeys.CcToken, ccToken);

            return RedirectToPage("/Purchase/Confirm");
        }

    }
}