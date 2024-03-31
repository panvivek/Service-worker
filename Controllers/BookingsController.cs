using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;

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
              return _context.Booking != null ? 
                          View(await _context.Booking.ToListAsync()) :
                          Problem("Entity set 'ServiceWorkerWebsiteContext.Booking'  is null.");
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create(int? workerId)
        {
            // Check if a worker ID is provided
            if (workerId.HasValue)
            {
                // Load the worker with the associated service from the database
                var worker = await _context.Worker_List.Include(w => w.Service)
                                                       .FirstOrDefaultAsync(w => w.Worke_Id == workerId.Value);

                // Check if the worker exists
                if (worker == null)
                {
                    return NotFound("Worker not found.");
                }

                // Pass worker and service information to the view via ViewBag
                ViewBag.WorkerName = worker.Name;
                ViewBag.Speciality = worker.Speciality; // Make sure your Worker model has a navigation property to Service
                ViewBag.Price = worker.Price; 
                // Create a new booking instance pre-filled with the worker's ID
                var bookingModel = new Booking
                {
                    Worke_Id = workerId.Value,
                    // Ensure you have a Service_Id property in your Booking model
                };

                return View(bookingModel);
            }

            // If no worker ID is provided, just return the view
            return View(new Booking());
        }







        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceId,WorkerId,BookingDate,CustomerName,CustomerEmail,AgreeToTerms,BookingTime")] Booking booking)
        {

            
            // Check if the terms and conditions are agreed upon
            if (!booking.AgreeToTerms)
            {
                ModelState.AddModelError("AgreeToTerms", "You must agree to the terms and conditions.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                // Redirect to a confirmation page or a success message
                return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
            }

            // If there are validation errors, repopulate the dropdowns and return to the form
           

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
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceId,WorkerId,BookingDate")] Booking booking)
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
            if (_context.Booking == null)
            {
                return Problem("Entity set 'ServiceWorkerWebsiteContext.Booking'  is null.");
            }
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
          return (_context.Booking?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Confirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Service_Id)  // Assuming Service is a navigation property
                .Include(b => b.Worke_Id)   // Assuming Worker is a navigation property
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }



    }
}
