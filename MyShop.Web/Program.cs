using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.DataAccess.Implementaion;
using myshop.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using myshop.Entities.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using myshop.Utilities;
using Stripe;
using myshop.DataAccess.DbInitializer;

namespace MyShop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 1- Bulid in services and already register in IOC container "Iconfiguration"
            // 2- Bulid in services but not register in IOC container "AddSession"
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            //.AddSessionStateTempDataProvider();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(o =>
            {
                //o.IdleTimeout = TimeSpan.FromMinutes(20);
                // to Add More Options
            });

            builder.Services.AddDbContext<AppDbContext>(optionBulider =>
            {
                optionBulider.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(op =>
            {
                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(4);
            })
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
            //    option=> {
            //      //  option.Password.RequireNonAlphanumeric = true;
            //        })
            //    .AddEntityFrameworkStores<AppDbContext>();


            //3- custom service "register type -singleton -transient -scoped"
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Value;//Get<string>();

            SeedDb();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void SeedDb()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var DbInsitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    DbInsitializer.Initialize();
                }
            }
        }


    }
}
