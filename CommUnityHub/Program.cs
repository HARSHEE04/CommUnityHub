using CommUnityHub.Data;
using CommUnityHub.Models;
using CommUnityHub.Services;
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

        
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
