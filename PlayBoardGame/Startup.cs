using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayBoardGame.Models;
using PlayBoardGame.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;

namespace PlayBoardGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => _configuration = configuration;

        public IConfiguration _configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<ApplicationDBContext>(options => 
            options.UseSqlServer(_configuration["Data:PlayBoardGame:ConnectionString"]));
            services.AddTransient<IGameRepository, EFGameRepository>();
            services.AddTransient<IShelfRepository, EFShelfRepository>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IEmailTemplateSender, EmailTemplateSender>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ContextProvider>();
            services.AddIdentity<AppUser, IdentityRole>(opts => {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
            SeedData.EnsurePopulatedAsync(app, _configuration);
        }
    }
}
