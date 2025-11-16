using CommUnityHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CommUnityHub.Data

/*Class developed by Harsheta Sharma
 * The purpose of this file is to define the ResourceDbContext class that
 * has the connection to the SQL Server database and represents the Resource table
 * 
 */
{
    public class ResourceDbContext: DbContext
    {

        public ResourceDbContext(DbContextOptions<ResourceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
    }
}
