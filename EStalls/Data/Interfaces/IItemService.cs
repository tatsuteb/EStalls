using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;
using Microsoft.AspNetCore.Http;

namespace EStalls.Data.Interfaces
{
    public interface IItemService
    {
        Task<Item> GetItemAsync(Guid id);
        Task<Item[]> GetRegisteredItemsAsync();

        /// <summary>
        /// 作品の基本情報をDBに保存する
        /// Id、RegistrationTime、UpdateTimeは、このメソッドの中で生成している
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddItemAsync(Item item);

        Task UpdateItemAsync(Item item);
    }
}
