using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PlayBoardGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Game}/{action=List}/{id?}");
            });
        }
    }
}
