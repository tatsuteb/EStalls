using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface ICartService
    {
        IEnumerable<Cart> GetAll();
        Guid GetId(Guid userId);
        Task AddAsync(Cart cart);
    }
}
