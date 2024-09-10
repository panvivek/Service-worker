using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;


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

            var applicationDbContext = _context.Booking.Include(b => b.Service);

            return View(await applicationDbContext.ToListAsync());

        }

        // GET: Bookings/Details/5

        public async Task<IActionResult> Details(int? id)

        {

            if (id == null || _context.Booking == null)

            {

                return NotFound();

            }

            var booking = await _context.Booking

                .Include(b => b.Service)

                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)

            {

                return NotFound();

            }

            return View(booking);

        }

        public IActionResult Create(int workerId, int serviceId)

        {

            ViewData["Service_Id"] = serviceId;

            ViewData["Worker_Id"] = workerId;

            var booking = new Booking();

            // Limit booking date to 7 days from now

            booking.BookingDate = DateTime.Today.AddDays(7);


            var availableTimeSlots = _context.TimeSlot_List

                                     .Where(ts => ts.Worker_Id == workerId && !ts.IsBooked

&& ts.StartTime.Date >= DateTime.Today

&& ts.StartTime.Date <= DateTime.Today.AddDays(7))

                                     .ToList();


            // Pass the available time slots to the view

            ViewData["AvailableTimeSlots"] = new SelectList(availableTimeSlots, "TimeSlotId", "StartTime");

            return View(booking);

        }

        // POST: Bookings/Create

        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Service_Id,Worker_Id,BookingDate,CustomerName,CustomerContact,AgreeToTerms,BookingTime,TimeSlotId")] Booking booking)

        {

            if (ModelState.IsValid)

            {

                // Add the booking to the context

                _context.Add(booking);

                await _context.SaveChangesAsync();

                // Retrieve the booked TimeSlot from the database using the TimeSlotId from the Booking object

                var bookedTimeSlot = await _context.TimeSlot_List.FirstOrDefaultAsync(ts => ts.TimeSlotId == booking.TimeSlotId);

                if (bookedTimeSlot != null)

                {

                    // Update the IsBooked property to true

                    bookedTimeSlot.IsBooked = true;

                    // Update the TimeSlot in the database

                    _context.Update(bookedTimeSlot);

                    await _context.SaveChangesAsync();

                }

                // Redirect to the Index action

                return RedirectToAction("Index", "Paypal");

            }

            // If we get to this point, something went wrong; re-display the form

            return View(booking);

        }

        [HttpPost]
        public async Task<IActionResult> GetTimeSlotsForDate([FromBody] TimeSlotRequest request)
        {
            var availableTimeSlots = await _context.TimeSlot_List
                .Where(ts => ts.Worker_Id == request.WorkerId
                           && !ts.IsBooked
                           && ts.StartTime.Date == request.Date.Date)
                .Select(ts => new { value = ts.TimeSlotId, text = ts.StartTime.ToString("HH:mm") })
                .ToListAsync();

            if (!availableTimeSlots.Any())
            {
                Console.WriteLine("No available time slots for the selected date.");
                return Json(new { message = "Please select a different date, no slots available for the selected date." });
            }

            Console.WriteLine("Available time slots:");
            foreach (var timeSlot in availableTimeSlots)
            {
                Console.WriteLine($"Time Slot ID: {timeSlot.value}, Time: {timeSlot.text}");
            }

            return Json(availableTimeSlots);
        }



        // Helper class to deserialize the request

        public class TimeSlotRequest

        {

            public DateTime Date { get; set; }

            public int WorkerId { get; set; }

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Service_Id,Worker_Id,BookingDate,CustomerName,CustomerEmail,AgreeToTerms,BookingTime")] Booking booking)

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

                return Problem("Entity set 'ApplicationDbContext.Booking'  is null.");

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

            return _context.Booking.Any(e => e.Id == id);

        }

    }

}
