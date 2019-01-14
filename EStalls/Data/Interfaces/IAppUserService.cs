using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data.Models;

namespace EStalls.Data.Interfaces
{
    public interface IAppUserService
    {
        IEnumerable<AppUser> GetAll();
        AppUser Get(string id);
        Task<AppUser> GetAsync(string id);
    }
}
