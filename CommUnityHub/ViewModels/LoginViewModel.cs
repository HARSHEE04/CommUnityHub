using System.ComponentModel.DataAnnotations;

namespace CommUnityHub.ViewModels
{
    // ViewModel for Login specifying the data needed for Login
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        // Password will appear as dots in view
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
