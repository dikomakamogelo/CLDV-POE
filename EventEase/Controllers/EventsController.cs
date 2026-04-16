using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Include(e => e.Venue).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventName,Description,StartDate,EndDate,VenueId,ImageUrl")] Event evt)
        {
            if (ModelState.IsValid)
            {
                if (evt.EndDate <= evt.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be after the start date.");
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
                    return View(evt);
                }
                _context.Add(evt);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Event '{evt.EventName}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            return View(evt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.Events.FindAsync(id);
            if (evt == null) return NotFound();
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,StartDate,EndDate,VenueId,ImageUrl")] Event evt)
        {
            if (id != evt.EventId) return NotFound();
            if (ModelState.IsValid)
            {
                if (evt.EndDate <= evt.StartDate)
                {
                    ModelState.AddModelError("EndDate", "End date must be after the start date.");
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
                    return View(evt);
                }
                try
                {
                    _context.Update(evt);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Event '{evt.EventName}' was updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.EventId == evt.EventId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            return View(evt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null) return NotFound();
            if (evt.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete this event because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }
            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Event '{evt.EventName}' was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}