using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext _context;

        public OrderItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OrderItem orderItem)
        {
            await _context.OrderItem
                .AddAsync(orderItem);

            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<OrderItem> orderItems)
        {
            await _context.OrderItem
                .AddRangeAsync(orderItems);

            await _context.SaveChangesAsync();
        }

        public IEnumerable<OrderItem> GetAll()
        {
            return _context.OrderItem;
        }
    }
}
