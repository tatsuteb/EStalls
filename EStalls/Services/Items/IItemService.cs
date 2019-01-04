using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Http;

namespace EStalls.Services.Items
{
    public interface IItemService
    {
        Task<Item[]> GetRegisteredItemsAsync();
        Task SaveItemAsync(InputItemModel inputItem);
    }
}
