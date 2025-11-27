using CommUnityHub.Models;
using CommUnityHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommUnityHub.Controllers
{
    // Allows controller to be accessed by non-logged-in users
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // Manager for handling user data, creation, and role assignment
        private readonly UserManager<User> _userManager;
        // Manager for handling user sign-in and sign-out operations
        private readonly SignInManager<User> _signInManager;

        // Initializes the controller with required Identity services
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Account Registration - Displays the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Account Registration - Handles form submission for new user creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Checks if data annotations pass validation
            if (!ModelState.IsValid)
                return View(model);

            // Creates a new User object from the custom IdentityUser class.
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                // Volunteers must be verified by an Admin, so IsVerified is initially false
                IsVolunteer = false,
                IsVerified = false,
                IsSystemAdmin = false
            };
            // Attempts to hash the password and create the user in the database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the default role "CommunityMember"
                await _userManager.AddToRoleAsync(user, "CommunityMember");
                // Automatically signs the new user in upon successful creation
                await _signInManager.SignInAsync(user, isPersistent: false);
                // isPersistent: false means the session ends when the browser closes
                return RedirectToAction("loadDashboard", "Resources");
            }
            // If creation failed (e.g., duplicate email/username), adds Identity errors to ModelState
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            // Returns the view with errors displayed
            return View(model);
        }

        // Login - Displays the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login - Handles form submission for user authentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Checks if data annotations pass validation
            if (!ModelState.IsValid)
                return View(model);

            // Attempts to sign the user in using email (which is also the username) and password
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                // Redirects to the main dashboard after successful login.
                return RedirectToAction("loadDashboard", "Resources");
            // Adds a generic error message for invalid attempts
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // Logout - Handles user sign-out.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Signs out the current user and clears the authentication
            await _signInManager.SignOutAsync();
            // Redirects to the dashboard (accessible to guests) after logging out
            return RedirectToAction("loadDashboard", "Resources");
        }
    }
}