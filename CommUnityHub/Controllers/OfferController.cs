using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommUnityHub.Data;
using CommUnityHub.Models;

namespace CommUnityHub.Controllers
{
    public class OfferController : Controller
    {
        private readonly ResourceDbContext _context;

        public OfferController(ResourceDbContext context)
        {
            _context = context;
        }

        // GET: /Offer
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers.ToListAsync();
            return View(offers);
        }

        // GET: /Offer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Offer/Create
        [HttpPost]
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
    }
}
