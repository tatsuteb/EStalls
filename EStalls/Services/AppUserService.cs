using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;

namespace EStalls.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly ApplicationDbContext _context;

        public AppUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AppUser> GetAll()
        {
            return _context.Users;
        }
    }
}
