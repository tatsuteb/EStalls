using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EStalls.Data.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string DisplayName { get; set; }
    }
}
