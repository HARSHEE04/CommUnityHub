using CommUnityHub.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace CommUnityHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ResourceDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();
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
