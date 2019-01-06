using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using EStalls.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EStalls.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<ItemService> _logger;

        public ItemService(
            IHostingEnvironment environment,
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ItemService> logger)
        {
            _environment = environment;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<Item> GetAsync(Guid id)
        {
            return await _context.Item
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IEnumerable<Item> GetAll()
        {
            return _context.Item;
        }

        public IEnumerable<Item> GetByUid(string uid)
        {
            var items = _context.Item
                .Where(x => x.Uid == uid);

            return items;
        }

        public async Task AddAsync(Item item)
        {
            var dateTime = DateTime.Now;

            item.Id = Guid.NewGuid();
            item.RegistrationTime = dateTime;
            item.UpdateTime = dateTime;
            
            await _context.Item
                .AddAsync(item);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Item item)
        {
            var itemToUpdate = await _context.Item
                .FirstOrDefaultAsync(x => x.Id == item.Id);

            if (itemToUpdate == null)
            {
                return;
            }

            // 変更があった項目を上書きする
            if (!string.IsNullOrEmpty(item.Title) &&
                itemToUpdate.Title != item.Title)
            {
                itemToUpdate.Title = item.Title;
            }

            if (!string.IsNullOrEmpty(item.Description) &&
                itemToUpdate.Description != item.Description)
            {
                itemToUpdate.Description = item.Description;
            }

            if (itemToUpdate.Price != item.Price)
            {
                itemToUpdate.Price = item.Price;
            }

            if (!string.IsNullOrEmpty(item.PreviewFileNames) &&
                itemToUpdate.PreviewFileNames != item.PreviewFileNames)
            {
                itemToUpdate.PreviewFileNames = item.PreviewFileNames;
            }

            if (!string.IsNullOrEmpty(item.ThumbnailFileName) &&
                itemToUpdate.ThumbnailFileName != item.ThumbnailFileName)
            {
                itemToUpdate.ThumbnailFileName = item.ThumbnailFileName;
            }

            itemToUpdate.UpdateTime = DateTime.Now;


            await _context.SaveChangesAsync();
        }
    }
}
