using CommUnityHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CommUnityHub.Data
{
    // UserIdentity DBContext for User Migrations
    public class UserIdentityDbContext : IdentityDbContext<User>
    {
        public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
