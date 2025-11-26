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

            // Register DbContext
            builder.Services.AddDbContext<ResourceDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register CSV importer
            builder.Services.AddScoped<CSVImporter>();

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
                    importer.ImportResourcesFromCSV(
                        @"C:\Users\Harsh\source\repos\CommUnityHub\CommUnityHub\Data\CommunityServices.csv"
                    );

                    Console.WriteLine(">>> IMPORT COMPLETE <<<");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(">>> IMPORT FAILED <<<");
                    Console.WriteLine(ex.ToString());  // << prints full error for debugging
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Seed admin user
                var userContext = services.GetRequiredService<UserIdentityDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                DBInitializer.Initialize(userContext, userManager);
            }


            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller=Resources}/{action=loadDashboard}/{id?}"
            );

            app.Run();
        }
    }
}
