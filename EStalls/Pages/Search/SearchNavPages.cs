using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Pages.Search
{
    public class SearchNavPages
    {
        public static string Items => "Items";
        public static string Users => "Users";

        public static string ItemsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Items);
        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
