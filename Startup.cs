using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CrashBoard.Models;
using Microsoft.EntityFrameworkCore;
using Amazon.SecretsManager;
using Amazon;
using Microsoft.AspNetCore.Identity;

namespace CrashBoard
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
            services.AddControllersWithViews();

            services.AddDbContext<CrashDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:RDSConnection"]);
            });

            services.AddDbContext<AppIdentityDBContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:RDSConnection"]);
            });

            // Added this to enable cookies
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddScoped<ICrashRepository, EFCrashRepository>();

            //enables HSTS??
            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(365);
            //});

            //Password Properties -- these are the default
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDBContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains().Preload());
            }

            //app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains().Preload());
            //app.UseHsts();
            app.UseHttpsRedirection();

            //content security policy (CSP) HTTP header 
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' cdn.jsdelivr.net 'unsafe-inline';");
                await next();
            });

            app.UseStaticFiles();

            // Added this to enable cookies
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("SearchPage", "{severity}/{pageNum}/{searchString}",
                    new { Controller = "Home", action = "CrashData" });

                endpoints.MapControllerRoute("SeverityPage", "{severity}/{pageNum}",
                    new { Controller = "Home", action = "CrashData" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
