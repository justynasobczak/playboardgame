using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayBoardGame.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;

namespace PlayBoardGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        private IConfiguration _configuration { get; }
        private IHostingEnvironment _environment { get; }
        private string _connectionString { get; set; }


        public void ConfigureServices(IServiceCollection services)
        {
            var serverDocker = _configuration["DBServer"];
            var portDocker = _configuration["DBPort"];
            var userDocker = _configuration["DBUser"];
            var passwordDocker = _configuration["DBPassword"];
            var databaseDocker = _configuration["DBDatabase"];

            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            if (_environment.IsDevelopment())
            {
                _connectionString = _configuration["PlayBoardGame:ConnectionString"];
            }

            if (_environment.IsProduction())
            {
                _connectionString = _configuration.GetConnectionString("GameetProd");
            }

            if (_environment.EnvironmentName == "Docker")
            {
                _connectionString =
                    $"Server={serverDocker},{portDocker};Initial Catalog={databaseDocker};User ID={userDocker};Password={passwordDocker}";
            }


            /*_connectionString = _environment.IsDevelopment()
                ? _configuration["PlayBoardGame:ConnectionString"]
                : _configuration.GetConnectionString("GameetProd");*/

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(_connectionString));
            services.AddTransient<IGameRepository, EFGameRepository>();
            services.AddTransient<IShelfRepository, EFShelfRepository>();
            services.AddTransient<IMeetingRepository, EFMeetingRepository>();
            services.AddTransient<IInvitedUserRepository, EFInvitedUserRepository>();
            services.AddTransient<IMessageRepository, EFMessageRepository>();
            services
                .AddTransient<ITomorrowsMeetingsNotificationRepository, EFTomorrowsMeetingsNotificationRepository>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.AddTransient<IEmailTemplateSender, EmailTemplateSender>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMiniProfiler()
                .AddEntityFramework();
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = _configuration["GoogleClientId"];
                options.ClientSecret = _configuration["GoogleClientSecret"];
            });
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
                app.UseMiniProfiler();
            }

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