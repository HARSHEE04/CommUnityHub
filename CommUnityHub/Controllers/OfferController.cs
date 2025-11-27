using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommUnityHub.Data;
using CommUnityHub.Models;
using Microsoft.AspNetCore.Authorization;

namespace CommUnityHub.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private readonly ResourceDbContext _context;

        public OfferController(ResourceDbContext context)
        {
            _context = context;
        }

        // PUBLIC LISTING: shows only Approved offers
        // GET: /Offer
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Where(o => o.Status == "Approved")
                .ToListAsync();

            return View(offers);
        }

        // GET: /Offer/Create
        [Authorize(Roles = "Volunteer,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Offer/Create
        [HttpPost]
        [Authorize(Roles = "Volunteer,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (!ModelState.IsValid)
            {
                return View(offer);
            }

            offer.Status = "Pending";
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET ADMIN: /Offer/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST ADMIN: /Offer/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.OfferId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(offer);
            }

            try
            {
                _context.Update(offer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(offer.OfferId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET ADMIN: /Offer/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .FirstOrDefaultAsync(m => m.OfferId == id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST ADMIN: /Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ADMIN: list all Pending offers
        // GET: /Offer/Pending
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pendingOffers = await _context.Offers
                .Where(o => o.Status == "Pending")
                .ToListAsync();

            return View(pendingOffers);
        }

        // ADMIN: Approve offer
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.Status = "Approved";
            _context.Update(offer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        // ADMIN: Reject offer
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.Status = "Rejected";
            _context.Update(offer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.OfferId == id);
        }
    }
}