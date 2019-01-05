using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface IItemDlInfoService
    {
        IEnumerable<ItemDlInfo> GetItemDlInfosByItemId(Guid itemId);

        /// <summary>
        /// ダウンロード情報をDBに保存する。
        /// Id、RegistrationTime、UpdateTimeは、このメソッドの中で生成している
        /// </summary>
        /// <param name="itemDlInfo"></param>
        /// <returns></returns>
        Task AddItemDlInfoAsync(ItemDlInfo itemDlInfo);
    }
}
