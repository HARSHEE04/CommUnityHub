using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CommUnityHub.Models;
using System.Linq; 

namespace CommUnityHub.Controllers
{
    // Authorization allows controller to be accessed by only users with the "Admin" role
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Manager for handling user data and making changes to users (roles, properties)
        private readonly UserManager<User> _userManager;

        // Initializes the controller with the required UserManager service
        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Admin Dashboard - Displays the list of users awaiting approval
        public IActionResult Dashboard()
        {

            // Queries the database (via UserManager) to find all users where the
            // custom IsVerified property is set to false (pending volunteers)
            var pendingUsers = _userManager.Users
                .Where(u => !u.IsVerified)
                .ToList();
            // returns view with list of pending volunteers
            return View(pendingUsers);
        }

        // Admin Approve Volunteer - Approves a pending user's volunteer request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVolunteer(string id)
        {
            // checks to make sure id passed through action is not null
            // otherwise returns NotFound Error
            if (string.IsNullOrEmpty(id))
                return NotFound();
            // Finds the target user by their unique ID.
            var user = await _userManager.FindByIdAsync(id);
            // If no user exists with specified id returns NotFound Error
            if (user == null)
                return NotFound();

            // Updates custom user properties to mark them as approved and a volunteer
            user.IsVerified = true;
            user.IsVolunteer = true;

            // Saves the changes to the user properties in the database
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                // If update fails, store an error message and redirect back to the dashboard.
                TempData["Error"] = $"Failed to approve volunteer {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Assign the user to the "Volunteer" Identity Role for role-based authorization checks
            await _userManager.AddToRoleAsync(user, "Volunteer");
            // Redirects to the dashboard with a success message.
            TempData["Success"] = $"{user.FullName} approved as Volunteer.";
            return RedirectToAction(nameof(Dashboard));
        }

        // Admin Reject User - Deletes a user/rejects an application.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectUser(string id)
        {
            // checks to make sure id passed through action is not null
            // otherwise returns NotFound Error
            if (string.IsNullOrEmpty(id))
                return NotFound();
            // Finds the target user by their unique ID.
            var user = await _userManager.FindByIdAsync(id);
            // If no user exists with specified id returns NotFound Error
            if (user == null)
                return NotFound();

            // Deletes the user account, including all related Identity data
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                // If update fails, store an error message and redirect back to the dashboard.
                TempData["Error"] = $"Failed to reject (delete) {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }
            // Redirects to the dashboard with a success message.
            TempData["Success"] = $"{user.FullName} has been removed.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}