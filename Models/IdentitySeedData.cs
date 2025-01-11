using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Wertyu5522-";
        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = adminUser,
                    FullName = "Çağatay Kalsan",
                    Email = "admin@cagatay.com",
                    PhoneNumber = "5554443322"
                };

                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
