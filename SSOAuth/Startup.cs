using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SSOAuth.Business;
using SSOAuth.Business.Interface;
using SSOAuth.Cache;
using SSOAuth.Data.Entity.Authorization;
using SSOAuth.Extensions;
using SSOAuth.Middleware;
using SSOAuth.Settings;
using static SSOAuth.Extensions.ConfigurationExtension;

namespace SSOAuth
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

            var config = Configuration.LoadConfiguration();
            services.AddSingleton<AppSettings>(config);

            services.AddDbContext<AuthDBContext>(options =>
            options.UseSqlServer(config.ConnectionStrings.AuthDB).UseLazyLoadingProxies());

            //Configuration.ReadConfiguration(configPath);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddMemoryCache();
            services.AddScoped<ICachingService, CachingService>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = false;
                  options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                  options.Cookie.SameSite = SameSiteMode.Lax;
              });


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = CookieSecurePolicy.None;
            });


            var context = services.BuildServiceProvider()
                      .GetService<AuthDBContext>();

            if (config.DataSettings.Migrate)
            {
                context.Database.Migrate();
            }
            if (config.DataSettings.Seed)
            {
                context.Seed(config);
            }
           
            services.AddControllers();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMiddleware<ExceptionHandelingMiddleware>();


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
}
