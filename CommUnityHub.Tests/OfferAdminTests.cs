using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using CommUnityHub.Data;
using CommUnityHub.Models;
using CommUnityHub.Controllers;

namespace CommUnityHub.Tests
{
    public class OfferAdminTests
    {
        // Use EF Core InMemory so tests don't touch the real DB
        private ResourceDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ResourceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())   // unique DB per test run
                .Options;

            return new ResourceDbContext(options);
        }

        [Fact]
        public async Task Pending_ReturnsOnlyPendingOffers()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            context.Offers.AddRange(
                new Offer { Title = "Pending 1", Category = "Test", ContactEmail = "a@test.com", Status = "Pending" },
                new Offer { Title = "Approved 1", Category = "Test", ContactEmail = "b@test.com", Status = "Approved" }
            );
            await context.SaveChangesAsync();

            var controller = new OfferController(context);

            // Act
            var result = await controller.Pending() as ViewResult;

            // Assert
            Assert.NotNull(result);

            var model = Assert.IsAssignableFrom<IEnumerable<Offer>>(result.Model);
            Assert.All(model, o => Assert.Equal("Pending", o.Status));
        }

        [Fact]
        public async Task Approve_ChangesStatusToApproved()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var offer = new Offer
            {
                Title = "Need Approval",
                Category = "Food",
                ContactEmail = "c@test.com",
                Status = "Pending"
            };

            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            var controller = new OfferController(context);

            // Act
            var result = await controller.Approve(offer.OfferId) as RedirectToActionResult;

            // Assert redirect
            Assert.NotNull(result);
            Assert.Equal("Pending", result.ActionName);   // redirects back to Pending view

            // Assert data change
            var updated = await context.Offers.FindAsync(offer.OfferId);
            Assert.NotNull(updated);
            Assert.Equal("Approved", updated.Status);
        }

        [Fact]
        public async Task Reject_ChangesStatusToRejected()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var offer = new Offer
            {
                Title = "Need Rejection",
                Category = "Other",
                ContactEmail = "d@test.com",
                Status = "Pending"
            };

            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            var controller = new OfferController(context);

            // Act
            var result = await controller.Reject(offer.OfferId) as RedirectToActionResult;

            // Assert redirect
            Assert.NotNull(result);
            Assert.Equal("Pending", result.ActionName);

            // Assert data change
            var updated = await context.Offers.FindAsync(offer.OfferId);
            Assert.NotNull(updated);
            Assert.Equal("Rejected", updated.Status);
        }
    }
}
