using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;

namespace PlayBoardGame.Models
{
    public static class SeedData
    {
        public static async void EnsurePopulatedAsync(IApplicationBuilder app, IConfiguration configuration)
        {
            var context = app.ApplicationServices
                .GetRequiredService<ApplicationDBContext>();
            context.Database.Migrate();

            var userManager = app.ApplicationServices.GetRequiredService<UserManager<AppUser>>();
            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();

            var username = configuration["Data:AdminUser:Name"];
            var email = configuration["Data:AdminUser:Email"];
            var password = configuration["Data:AdminUser:Password"];
            var role = configuration["Data:AdminUser:Role"];
            var timeZone = DateTimeZoneProviders.Tzdb.GetAllZones().FirstOrDefault().Id;

            if (await userManager.FindByNameAsync(username) == null)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                var user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    TimeZone = timeZone
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}