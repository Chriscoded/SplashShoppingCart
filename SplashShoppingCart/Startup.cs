using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SplashShoppingCart.Infrastructure;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromHours(2);
                //options.IdleTimeout = TimeSpan.FromDays(2);
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();
            services.AddDbContext<SplashShoppingCartContext>(options => options.UseSqlServer
            (Configuration.GetConnectionString("SplashShoppingCartDbConnection")));

            services.AddIdentity<AppUser, IdentityRole>(options => {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
                    .AddEntityFrameworkStores<SplashShoppingCartContext>()
                    .AddDefaultTokenProviders();

        //    services.AddSession(options =>
        //    {
        //    //Replace {mins} with the time you want, usually around 20-30.
        //    options.IdleTimeout = TimeSpan.FromDays(1);
        //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ////app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");

            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   "error",
                   "error/{statusCode}",
                   defaults: new {controller = "Error", action = "HttpStatusCodeHandler" }
                 );
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );

                endpoints.MapControllerRoute(
                    //controller name
                    "productscategory",
                    //route
                    "products/{categoryslug}",
                    defaults: new { controller = "Products", action = "ProductsByCategory" }
                    );

                endpoints.MapControllerRoute(
                   name: "products",
                   pattern: "{controller=Products}/{action=Index}"
                 );


                endpoints.MapControllerRoute(
                   //controller name
                   "pages",
                   //route
                   "{slug?}",
                   defaults: new { controller = "Pages", action = "Page" }
           );

                endpoints.MapControllerRoute(
                   name: "home",
                   pattern: "{controller=Home}/{action=Index}/{id?}"
                   );


            });
        }
    }
}
