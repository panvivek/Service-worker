using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceWorkerWebsite.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;



            public BookingsController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: Bookings
            public async Task<IActionResult> Index()
            {
                var bookings = await _context.Booking
                    .Include(b => b.Service)
                    .Include(b => b.Worker)
                    .ToListAsync();
                return View(bookings);
            }

        // GET: Bookings/Create
    
        public IActionResult Create(int workerId, int serviceId)
        {
            ViewData["Worker_Id"] = workerId;
            ViewData["Service_Id"] = serviceId;
            Console.WriteLine($"Worker ID: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}");

            return View();
        }
        // POST: Bookings/GetAvailableSlots
        [HttpPost]
        public async Task<JsonResult> GetAvailableSlots([FromBody] WorkerRequest request)
        {
            var workerId = request.WorkerId; // Accessing the workerId from the request object
            Console.WriteLine($"Entered GetAvailableSlots Method");
            Console.WriteLine($"Worker ID in GetAvailableSlots: {workerId}");

            var availableSlots = await _context.TimeSlot_List
                .Where(ts => ts.Worker_Id == workerId && !ts.IsBooked)
                .GroupBy(ts => ts.SelectedDates) // Group by date
                .Select(g => new
                {
                    date = g.Key, // The date
                    timeSlots = g.Select(ts => ts.TimeSlots).ToList() // List of time slots for that date
                })
                .ToListAsync();

            return Json(availableSlots);
        }


        // Define a class to deserialize the request
        public class WorkerRequest
        {
            public int WorkerId { get; set; }
        }

        // POST: Bookings/Create
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Worker_Id,BookingDate,CustomerName,CustomerContact,Service_Id,AgreeToTerms,TimeSlotId")] Booking booking)
            {
                if (ModelState.IsValid)
                {
                    var timeSlot = await _context.TimeSlot_List.FindAsync(booking.TimeSlotId);
                    if (timeSlot != null)
                    {
                        timeSlot.IsBooked = true;
                        _context.Update(timeSlot);
                    }

                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(booking);
            }

          
            // GET: Bookings/Edit/5
            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Service_Id,Worker_Id,BookingDate,CustomerName,CustomerContact,AgreeToTerms,TimeSlotId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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

            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }

    public class DateTimeRequest
    {
        public DateTime Date { get; set; }
        public int WorkerId { get; set; }
    }
}
