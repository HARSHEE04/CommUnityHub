using CommUnityHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CommUnityHub.Data
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
