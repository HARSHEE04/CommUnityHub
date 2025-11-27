using CommUnityHub.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CommUnityHub.Data
{
    public static class DBInitializer
    {
        // Add RoleManager<IdentityRole> to the method signature for User Roles to implement Authorization based on Roles
        public static async Task Initialize(UserIdentityDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Ensure Roles exist
            string[] roleNames = { "Admin", "Volunteer", "CommunityMember" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed system admin user
            string adminEmail = "admin@communityhub.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                var sysAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true,
                    IsSystemAdmin = true, 
                    IsVerified = true,
                    IsVolunteer = true
                };

                var createResult = await userManager.CreateAsync(sysAdmin, "Admin123!");

                if (createResult.Succeeded)
                {
                    // Assign the user to the "Admin" Identity Role
                    await userManager.AddToRoleAsync(sysAdmin, "Admin");
                }
            }
        }
    }
}