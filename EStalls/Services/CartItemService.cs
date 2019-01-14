using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ApplicationDbContext _context;

        public CartItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CartItem> GetAll()
        {
            return _context.CartItem;
        }

        public async Task AddAsync(CartItem cartItem)
        {
            await _context.CartItem
                .AddAsync(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItem
                .Update(cartItem);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(CartItem[] cartItems)
        {
            _context.CartItem
                .UpdateRange(cartItems);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid cartId, Guid itemId)
        {
            var item = _context.CartItem
                .SingleOrDefault(x => x.CartId == cartId && x.ItemId == itemId);

            if (item == null)
                return;

            _context.CartItem
                .Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}
