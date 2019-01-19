using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;
        
            await _context.Order
                .AddAsync(order);

            await _context.SaveChangesAsync();
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Order;
        }

        public IEnumerable<Order> GetByUid(Guid uid)
        {
            return _context.Order
                .Where(x => x.Uid == uid);
        }
    }
}
