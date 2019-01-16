using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface IOrderItemService
    {
        Task AddAsync(OrderItem orderItem);
        Task AddRangeAsync(IEnumerable<OrderItem> orderItems);
    }
}
