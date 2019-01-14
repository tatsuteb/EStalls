using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EStalls.Data;
using EStalls.Data.Interfaces;
using EStalls.Data.Models;
using Microsoft.EntityFrameworkCore;

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

        public AppUser Get(string id)
        {
            return _context.Users
                .SingleOrDefault(x => x.Id == id);
        }

        public async Task<AppUser> GetAsync(string id)
        {
            return await _context.Users
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
