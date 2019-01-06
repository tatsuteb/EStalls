using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class EditNavPages
    {
        public static string EditBaseInfo => "EditBaseInfo";
        public static string EditVersion => "EditVersion";

        public static string EditBaseInfoNavClass(ViewContext viewContext) => PageNavClass(viewContext, EditBaseInfo);
        public static string EditVersionNavClass(ViewContext viewContext) => PageNavClass(viewContext, EditVersion);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
