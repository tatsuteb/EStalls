using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface ICartItemService
    {
        IEnumerable<CartItem> GetAll();
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task UpdateRangeAsync(CartItem[] cartItems);
        Task Delete(Guid cartId, Guid itemId);
    }
}
