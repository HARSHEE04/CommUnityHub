using Microsoft.AspNetCore.Identity;

namespace CommUnityHub.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }   

        public bool IsVolunteer { get; set; } // Role-based 
        public bool IsVerified { get; set; } // Verified by Admin

        public bool IsSystemAdmin { get; set; } // Role-based
    }
}
