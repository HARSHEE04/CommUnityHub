using CommUnityHub.Data;
using CommUnityHub.Models;
using CommUnityHub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommUnityHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

       
            builder.Services.AddControllersWithViews();

            // Register DbContext for Resources
            builder.Services.AddDbContext<ResourceDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register CSV importer
            builder.Services.AddScoped<CSVImporter>();

            // // Register DbContext
            builder.Services.AddDbContext<UserIdentityDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DBStr")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserIdentityDbContext>()
                .AddDefaultTokenProviders();


            //Register ResourceManager service
            builder.Services.AddScoped<ResourceManager>();


            var app = builder.Build();

           
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    Console.WriteLine(">>> RUNNING CSV IMPORTER <<<");

                    var importer = scope.ServiceProvider.GetRequiredService<CSVImporter>();

                    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                    string csvPath = Path.Combine(env.ContentRootPath, "Data", "CommunityServices.csv");

                    importer.ImportResourcesFromCSV(csvPath);

                    Console.WriteLine(">>> IMPORT COMPLETE <<<");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(">>> IMPORT FAILED <<<");
                    Console.WriteLine(ex.ToString());  // << prints full error for debugging
                }
            }

            // Services for UserIdentityDbContext
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<UserIdentityDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); 
                // seed roles and admin data
                DBInitializer.Initialize(context, userManager, roleManager).GetAwaiter().GetResult();
            }

            // Middleware needed for Project to run
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller=Resources}/{action=loadDashboard}/{id?}"
            );

            app.Run();
        }
    }
}
