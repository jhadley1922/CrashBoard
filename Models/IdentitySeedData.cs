using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    public static class IdentitySeedData
    {
        private const string TAUser = "TAUser";
        private const string TAPassword = "Password654321!";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            AppIdentityDBContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<AppIdentityDBContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            UserManager<IdentityUser> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            IdentityUser user = await userManager.FindByIdAsync(TAUser);

            if (user == null)
            {
                user = new IdentityUser(TAUser);
                user.Email = "tauser@test.com";
                user.PhoneNumber = "555-5555";

                await userManager.CreateAsync(user, TAPassword);
            }
        }
    }
}

