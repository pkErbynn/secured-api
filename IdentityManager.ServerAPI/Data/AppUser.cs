using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityManager.ServerAPI.Data
{
    public class AppUser: IdentityUser
    {
        public string? Name { get; set; }
    }
}