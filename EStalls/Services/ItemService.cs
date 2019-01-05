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

        public async Task<Item[]> GetRegisteredItemsAsync()
        {
            var user = await _userManager.GetUserAsync(this._signInManager.Context.User);
            var uid = user.Id;

            var items = await _context.Item
                .AsNoTracking()
                .Where(x => x.Uid == uid)
                .ToArrayAsync();

            return items;
        }

        public async Task AddItemAsync(Item item)
        {
            var dateTime = DateTime.Now;

            item.Id = Guid.NewGuid();
            item.RegistrationTime = dateTime;
            item.UpdateTime = dateTime;
            
            var addedItem = await _context.Item
                .AddAsync(item);

            await _context.SaveChangesAsync();
        }
    }
}
