using System.ComponentModel.DataAnnotations;

namespace CommUnityHub.ViewModels
{
    // ViewModel for Registration specifying the data needed for User Registration
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        // Password will appear as dots in view
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        // Password will appear as dots in view
        [DataType(DataType.Password)]
        // Ensure ConfirmPassword matches Password
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }

        // Bool to determine if user is registering as a volunteer
        public bool IsVolunteer { get; set; } // User selects volunteer role
    }
}
