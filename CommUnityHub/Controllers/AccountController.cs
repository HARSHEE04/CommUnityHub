using CommUnityHub.Models;
using CommUnityHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommUnityHub.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Account Registration
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                IsVolunteer = false,
                IsVerified = false,
                IsSystemAdmin = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // CRITICAL FIX: Assign the default role "CommunityMember"
                // This role must be created in the DBInitializer.
                await _userManager.AddToRoleAsync(user, "CommunityMember");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("loadDashboard", "Resources");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Note: PasswordSignInAsync needs a User.UserName, which you set to Email in registration.
            // This is generally fine, but if you want to use Email directly, ensure your
            // Identity configuration (in Program.cs/Startup.cs) is set up for it.
            // Assuming your config allows sign-in by email/username (where email = username here).

            if (!ModelState.IsValid)
                return View(model);

            // Use the RememberMe property from the LoginViewModel (was hardcoded to false)
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("loadDashboard", "Resources");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("loadDashboard", "Resources");
        }
    }
}