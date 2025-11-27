using CommUnityHub.Controllers;
using CommUnityHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CommUnityHub.Tests
{
    // Helper class to mock Identity services required by UserManager
    public static class MockHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> users = null) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup for the Queryable property used in the Dashboard action
            if (users != null)
            {
                userManager.Setup(m => m.Users).Returns(users.AsQueryable());
            }
            // If 'users' is null, m.Users is NOT set up for LINQ querying.
            // This is fine for tests that don't query the list (like Approve/Reject).
            else
            {
                // Fallback setup for 'Users' property to prevent NullReferenceException if accessed unexpectedly
                userManager.Setup(m => m.Users).Returns(Enumerable.Empty<TUser>().AsQueryable());
            }

            return userManager;
        }

        public static AdminController CreateControllerWithTempData(Mock<UserManager<User>> userManagerMock)
        {
            var controller = new AdminController(userManagerMock.Object);
            // Setup TempData to test success/error messages
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            return controller;
        }

        // Creates a clean test user copy for each test
        public static User CreateTestUser(string id = "u1", string fullName = "Pending Volunteer")
        {
            return new User
            {
                Id = id,
                FullName = fullName,
                Email = "test@hub.ca",
                IsVerified = false,
                IsVolunteer = false
            };
        }
    }

    public class AdminControllerTest
    {
        // Sample list of users for the dashboard view test
        private readonly List<User> _testUsersList = new List<User>
        {
            new User { Id = "u1", FullName = "Pending Volunteer", Email = "pend@hub.ca", IsVerified = false, IsVolunteer = false },
            new User { Id = "u2", FullName = "Verified Member", Email = "verif@hub.ca", IsVerified = true, IsVolunteer = false },
            new User { Id = "u3", FullName = "Unrequested Member", Email = "anon@hub.ca", IsVerified = false, IsVolunteer = false }
        };

        // 1. DASHBOARD TEST (Verify pending users appear)

        [Fact]
        public void Dashboard_ReturnsViewWithPendingUsers()
        {
            // Arrange
            var userManagerMock = MockHelpers.MockUserManager(_testUsersList);
            var controller = new AdminController(userManagerMock.Object);

            // Act
            var result = controller.Dashboard();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<User>>(view.Model);

            // Expected: 2 unverified users (u1 and u3)
            Assert.Equal(2, model.Count());
        }

        // 2. APPROVE VOLUNTEER TEST (Verify user is validated and role is added)

        [Fact]
        public async Task ApproveVolunteer_ValidId_SetsFlagsAndRedirects()
        {
            // Arrange
            var userToApprove = MockHelpers.CreateTestUser(); // user object
            var userManagerMock = MockHelpers.MockUserManager<User>(null);

            // Mock necessary Identity calls for the success path
            userManagerMock.Setup(m => m.FindByIdAsync(userToApprove.Id)).ReturnsAsync(userToApprove);
            userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "Volunteer")).ReturnsAsync(IdentityResult.Success);

            var controller = MockHelpers.CreateControllerWithTempData(userManagerMock);

            // Act
            var result = await controller.ApproveVolunteer(userToApprove.Id);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AdminController.Dashboard), redirect.ActionName);

            // Verify model state changes on the object
            Assert.True(userToApprove.IsVerified);
            Assert.True(userToApprove.IsVolunteer);

            // Verify role assignment call occurred
            userManagerMock.Verify(m => m.AddToRoleAsync(userToApprove, "Volunteer"), Times.Once());
        }

        // 3. REJECT USER TEST (Verify user is deleted)

        [Fact]
        public async Task RejectUser_ValidId_DeletesUserAndRedirects()
        {
            // Arrange
            var userToReject = MockHelpers.CreateTestUser(); // user object

            var userManagerMock = MockHelpers.MockUserManager<User>(null);

            // Mock necessary Identity calls for the success path
            userManagerMock.Setup(m => m.FindByIdAsync(userToReject.Id)).ReturnsAsync(userToReject);
            userManagerMock.Setup(m => m.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var controller = MockHelpers.CreateControllerWithTempData(userManagerMock);

            // Act
            var result = await controller.RejectUser(userToReject.Id);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AdminController.Dashboard), redirect.ActionName);

            // Verify deletion call occurred
            userManagerMock.Verify(m => m.DeleteAsync(userToReject), Times.Once());
        }
    }
}