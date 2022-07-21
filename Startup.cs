using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using Intro.Middleware;
using Intro.Services;

namespace Intro
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
        { // services - контейнер, через который будут регистрироваться службы
            services.AddDbContext<DAL.Context.IntroContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("introDb")
            ));

            services.AddControllersWithViews();
            services.AddSingleton<RandomService>();
            services.AddSingleton<IHasher, ShaHasher>(); // можно сменить тип отображения данных на Md5Hasher
            // используем AddSingleton, т.к. у него максимальное время жизни и не требуется пересоздавать объект каждый раз
            // при каждом обращении получаем текущую дату и время
            services.AddSingleton<IDateTime, CurrentDateTime>(); // можно сменить тип отображения данных на UTCDateTime

            services.AddScoped<IAuthService, SessionAuthService>();

            #region Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            #endregion

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
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            // app.UseMiddleware<Middleware.SessionAuthMiddleware>();
            app.UseSessionAuth(); // для использования расширений using обязателен

            //app.UseRequestLocalization(options =>
            //{
            //    options.SupportedCultures = new List<CultureInfo>
            //    {   // ISO 639-1 -- ISO 3166-1
            //        new CultureInfo("uk-UA"),
            //        new CultureInfo("ru-UA"),
            //        new CultureInfo("en-US")
            //    };
            //    options.SupportedUICultures = options.SupportedCultures;
            //    options.SetDefaultCulture(options.SupportedCultures[0].Name);
            //    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
