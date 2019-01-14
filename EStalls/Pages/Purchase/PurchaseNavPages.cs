using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Pages.Purchase
{
    public class PurchaseNavPages
    {
        public static string Cart => "Cart";
        public static string Payment => "Payment";
        public static string Confirm => "Confirm";
        public static string Complete => "Complete";

        public static string CartNavClass(ViewContext viewContext) => PageNavClass(viewContext, Cart);
        public static string PaymentNavClass(ViewContext viewContext) => PageNavClass(viewContext, Payment);
        public static string ConfirmNavClass(ViewContext viewContext) => PageNavClass(viewContext, Confirm);
        public static string CompleteNavClass(ViewContext viewContext) => PageNavClass(viewContext, Complete);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "is-active" : null;
        }
    }
}
