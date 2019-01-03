using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EStalls.Services.Items
{
    public interface IItemService
    {
        Task SaveItemAsync(InputItemModel inputItem);
    }
}
