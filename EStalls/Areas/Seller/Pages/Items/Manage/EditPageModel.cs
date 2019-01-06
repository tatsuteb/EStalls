using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Areas.Seller.Pages.Items.Manage
{
    public class EditPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid ItemId { get; set; }
    }
}
