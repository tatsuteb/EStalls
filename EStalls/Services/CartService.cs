using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Cart> GetAll()
        {
            return _context.Cart;
        }

        public Guid GetId(Guid userId)
        {
            var cart = _context.Cart
                .FirstOrDefault(x => x.Uid == userId);

            return cart?.Id ?? Guid.Empty;
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Cart
                .AddAsync(cart);

            await _context.SaveChangesAsync();
        }
    }
}
