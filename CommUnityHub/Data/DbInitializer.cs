using CommUnityHub.Models;
using Microsoft.AspNetCore.Identity;

namespace CommUnityHub.Data
{
    public static class DBInitializer
    {
        public static void Initialize(UserIdentityDbContext context, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            // Seed system admin
            string adminEmail = "admin@communityhub.com";
            var admin = userManager.FindByEmailAsync(adminEmail).Result;
            if (admin == null)
            {
                var sysAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true,
                    IsSystemAdmin = true
                };
                userManager.CreateAsync(sysAdmin, "Admin123!").Wait();
            }
        }
    }
}
