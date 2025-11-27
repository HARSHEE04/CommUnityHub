using Microsoft.AspNetCore.Identity;

namespace CommUnityHub.Models
{
    // User class Inherits from IdentityUser classs the implements ASP.Net Identity Framework
    public class User : IdentityUser
    {
        public string FullName { get; set; }   
        public bool IsVolunteer { get; set; } // Role-based 
        public bool IsVerified { get; set; } // Volunteer Verified by Admin
        public bool IsSystemAdmin { get; set; } // Role-based
    }
}
