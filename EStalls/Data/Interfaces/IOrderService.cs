using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface IOrderService
    {
        Task AddAsync(Order order);
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetByUid(Guid uid);
    }
}
