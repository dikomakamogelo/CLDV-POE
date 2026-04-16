using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Venue '{venue.VenueName}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (id != venue.VenueId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Venue '{venue.VenueName}' was updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venues.Any(v => v.VenueId == venue.VenueId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return NotFound();
            if (venue.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete this venue because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }
            if (venue.Events.Any())
            {
                TempData["Error"] = "Cannot delete this venue because it has existing events.";
                return RedirectToAction(nameof(Index));
            }
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Venue '{venue.VenueName}' was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}