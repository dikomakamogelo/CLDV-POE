using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,BookingDate,CustomerName,CustomerEmail")] Booking booking)
        {
            
            ModelState.Remove("BookingReference");

            if (ModelState.IsValid)
            {
                var conflict = await _context.Bookings
                    .AnyAsync(b => b.VenueId == booking.VenueId
                              && b.BookingDate.Date == booking.BookingDate.Date);

                if (conflict)
                {
                    ModelState.AddModelError(string.Empty, "This venue is already booked on the selected date. Please choose a different date or venue.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
                    return View(booking);
                }

                booking.BookingReference = $"BK-{DateTime.Now.Year}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Booking '{booking.BookingReference}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingReference,EventId,VenueId,BookingDate,CustomerName,CustomerEmail")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();
            if (ModelState.IsValid)
            {
                var conflict = await _context.Bookings
                    .AnyAsync(b => b.VenueId == booking.VenueId
                              && b.BookingDate.Date == booking.BookingDate.Date
                              && b.BookingId != booking.BookingId);
                if (conflict)
                {
                    ModelState.AddModelError(string.Empty, "This venue is already booked on the selected date.");
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
                    return View(booking);
                }
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(b => b.BookingId == booking.BookingId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}