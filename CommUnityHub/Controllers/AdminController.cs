using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CommUnityHub.Models;


namespace CommUnityHub.Controllers
{
    [Authorize(Roles = "SystemAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Dashboard showing all users awaiting verification
        public IActionResult Dashboard()
        {
            var pendingUsers = _userManager.Users
                .Where(u => !u.IsVerified)
                .ToList();

            return View(pendingUsers); // pass list to view
        }

        // Approve a user as volunteer
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

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.FullName} has been approved as a volunteer.";
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                TempData["Error"] = $"Error approving {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // Reject user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // You can delete or just mark as rejected
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.FullName} has been rejected.";
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                TempData["Error"] = $"Error rejecting {user.FullName}.";
                return RedirectToAction(nameof(Dashboard));
            }
        }
    }
}
