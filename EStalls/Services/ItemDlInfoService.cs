using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class ItemDlInfoService : IItemDlInfoService
    {
        public readonly ApplicationDbContext _context;

        public ItemDlInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddItemDlInfoAsync(ItemDlInfo itemDlInfo)
        {
            var dateTime = DateTime.Now;

            itemDlInfo.Id = Guid.NewGuid();
            itemDlInfo.RegistrationTime = dateTime;
            itemDlInfo.UpdateTime = dateTime;

            await _context.ItemDlInfo
                .AddAsync(itemDlInfo);

            await _context.SaveChangesAsync();
        }
    }
}
