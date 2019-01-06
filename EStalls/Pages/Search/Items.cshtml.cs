using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EStalls.Pages.Search
{
    public class ItemsModel : SearchPageModel
    {
        private readonly IItemService _itemService;

        public ItemsModel(
            IItemService itemService)
        {
            _itemService = itemService;
        }

        public IEnumerable<Item> Items { get; set; }

        public void OnGet()
        {
            var items = _itemService.GetAll();

            if (string.IsNullOrWhiteSpace(Keyword))
            {
                Items = items.ToArray();
                return;
            }

            Items = items.Where(x => x.Title.Contains(Keyword) || x.Description.Contains(Keyword))
                .ToArray();
        }
    }
}