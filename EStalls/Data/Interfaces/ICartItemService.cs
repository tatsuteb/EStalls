﻿using System;
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
        Task DeleteAsync(Guid cartId, Guid itemId);
        Task DeleteAsync(CartItem cartItem);
        Task DeleteRangeAsync(IEnumerable<CartItem> cartItems);
    }
}
