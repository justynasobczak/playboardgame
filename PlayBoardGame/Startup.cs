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
using Newtonsoft.Json;
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
            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(_configuration["PlayBoardGame:ConnectionString"]));
            services.AddTransient<IGameRepository, EFGameRepository>();
            services.AddTransient<IShelfRepository, EFShelfRepository>();
            services.AddTransient<IMeetingRepository, EFMeetingRepository>();
            services.AddTransient<IInvitedUserRepository, EFInvitedUserRepository>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IEmailTemplateSender, EmailTemplateSender>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ContextProvider>();
            services.AddMiniProfiler()
                .AddEntityFramework();
            services.AddIdentity<AppUser, IdentityRole>(opts =>
                {
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

            app.UseMiniProfiler();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
            SeedData.EnsurePopulatedAsync(app, _configuration);
        }
    }
}