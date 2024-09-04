using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Utilities;

namespace myshop.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        void Initialize();
    }

    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext context;

        public DbInitializer(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,AppDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }
        public void Initialize()
        {
            // Migration
            try
            {
                if (context.Database.GetPendingMigrations().Count() > 0)
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }
            // Roles
            if (!roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
                //User
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName="Admin@myshop.com",
                    Email= "Admin@myshop.com",
                    Name="Administrator",
                    PhoneNumber="01234567891",
                    Address= "Shubra",
                    City="Cairo",
                },"Admin.123").GetAwaiter().GetResult();

                ApplicationUser user=context.ApplicationUsers.FirstOrDefault(e=>e.Email== "Admin@myshop.com");
               
                if(user != null) 
                userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();

            }

            return;
        }
    }
}
