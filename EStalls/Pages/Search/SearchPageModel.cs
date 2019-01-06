using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Search
{
    public class SearchPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }
    }
}
