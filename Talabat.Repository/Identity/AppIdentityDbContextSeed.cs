using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            var user = new AppUser()
            {
                DisplayName= "Mohamed Gamal",
                Email= "mohamedGamal@gmail.com",
                UserName= "Mohamed.Gamal",
                PhoneNumber= "01204138365",
            };
            await _userManager.CreateAsync(user,"Mo@@200300");
        }
    }
}
