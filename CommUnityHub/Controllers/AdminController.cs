using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CommUnityHub.Models;
using System.Linq; // Required for .Where() and .ToList()

namespace CommUnityHub.Controllers
{
    // FIX: Change role name from "SystemAdmin" to the standardized "Admin"
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Note: The AdminController needs access to the System.Linq namespace
            // for the .Where() and .ToList() extensions, which I've added above.

            // This retrieves users who haven't been verified yet (potential volunteers)
            var pendingUsers = _userManager.Users
                .Where(u => !u.IsVerified)
                .ToList();

            return View(pendingUsers);
        }

        // POST: /Admin/ApproveVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVolunteer(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsVerified = true;
            user.IsVolunteer = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                TempData["Error"] = $"Failed to approve volunteer {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Add user to Volunteer role
            // This assumes the user being approved is currently only in the "CommunityMember" role.
            // If they are not, you might need logic to remove the old role first, but typically
            // a user can be in multiple roles.
            await _userManager.AddToRoleAsync(user, "Volunteer");

            TempData["Success"] = $"{user.FullName} approved as Volunteer.";
            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Admin/RejectUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Delete the account
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = $"Failed to reject (delete) {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }

            TempData["Success"] = $"{user.FullName} has been removed.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}