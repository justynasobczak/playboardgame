using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlayBoardGame.Models
{
    public static class SeedData
    {
        public static async void EnsurePopulatedAsync(IApplicationBuilder app, IConfiguration configuration)
        {
            ApplicationDBContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDBContext>();
            context.Database.Migrate();
           
            UserManager<AppUser> userManager = app.ApplicationServices.GetRequiredService<UserManager<AppUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();

            string username = configuration["Data:AdminUser:Name"];
            string email = configuration["Data:AdminUser:Email"];
            string password = configuration["Data:AdminUser:Password"];
            string role = configuration["Data:AdminUser:Role"];

            if (await userManager.FindByNameAsync(username) == null)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                AppUser user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    Country = Country.None
                    
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
