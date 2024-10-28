using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Areas.Identity.Data;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceWorkerWebsite.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;


        public BookingsController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Include related entities for Worker, Service, and TimeSlot
            var bookings = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker) // Include Worker to access UserId
                .ThenInclude(w => w.User) // Include User to access first and last name
                .Include(b => b.TimeSlot) // Include TimeSlot to display appointment details
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(bookings);
        }




        // GET: Bookings/Create

        public IActionResult Create(int workerId, int serviceId)
        {
            // Log the serviceId and workerId values
            Console.WriteLine($"Worker ID: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}");

            ViewData["Worker_Id"] = workerId;
            ViewData["Service_Id"] = serviceId;

            return View();
        

    }


    [HttpPost]
public async Task<JsonResult> GetAvailableSlots([FromBody] WorkerRequest request)
{
    var workerId = request.WorkerId; // Accessing the workerId from the request object
    var serviceId = request.ServiceId; // Accessing the serviceId from the request object
    Console.WriteLine($"Entered GetAvailableSlots Method");
    Console.WriteLine($"Worker ID: {workerId}");
    Console.WriteLine($"Service ID: {serviceId}"); // Log the serviceId

    // Fetch available slots and sort them
    var availableSlots = await _context.TimeSlot_List
        .Where(ts => ts.Worker_Id == workerId && !ts.IsBooked)
        .GroupBy(ts => ts.SelectedDates)
        .Select(g => new
        {
            date = g.Key,
            timeSlots = g.OrderBy(ts => ts.TimeSlots).Select(ts => new
            {
                TimeSlotId = ts.TimeSlotId,
                TimeSlots = ts.TimeSlots
            }).ToList()
        })
        .OrderBy(group => group.date) // Sort groups by date
        .ToListAsync();

    // Debugging: Print available slots to the console
    if (availableSlots.Count == 0)
    {
        Console.WriteLine("No available slots found.");
    }
    else
    {
        foreach (var group in availableSlots)
        {
            Console.WriteLine($"Date: {group.date}");
            foreach (var slot in group.timeSlots)
            {
                Console.WriteLine($"TimeSlotId: {slot.TimeSlotId}, TimeSlots: {slot.TimeSlots}");
            }
        }
    }

    return Json(availableSlots);
}



        // Fetching current user booked time slots and all avialble timeslots 

        [HttpPost]
        public async Task<JsonResult> EditAvailableSlots([FromBody] WorkerRequest request)
        {
            var workerId = request.WorkerId; // Accessing the workerId from the request object
            var serviceId = request.ServiceId; // Accessing the serviceId from the request object
            var timeSlotId = request.TimeSlotId; // Accessing the timeSlotId from the request object
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user ID
            Console.WriteLine($"Entered EditAvailableSlots Method");
            Console.WriteLine($"Worker ID: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}"); // Log the serviceId
            Console.WriteLine($"timeslot ID: {timeSlotId}"); // Log the serviceId

            // Fetch available slots and sort them
            var availableSlots = await _context.TimeSlot_List
                .Where(ts => ts.Worker_Id == workerId && !ts.IsBooked)
                .GroupBy(ts => ts.SelectedDates)
                .Select(g => new
                {
                    date = g.Key,
                    timeSlots = g.OrderBy(ts => ts.TimeSlots).Select(ts => new
                    {
                        TimeSlotId = ts.TimeSlotId,
                        TimeSlots = ts.TimeSlots
                    }).ToList()
                })
                .OrderBy(group => group.date) // Sort groups by date
                .ToListAsync();

            // Fetch the current booking for the logged-in user using TimeSlotId
            var currentBooking = await _context.Booking
                .Where(b => b.UserId == currentUserId && b.Worker_Id == workerId && b.Service_Id == serviceId && b.TimeSlotId == timeSlotId)
                .Select(b => new
                {
                    // Change BookingDate to the selected date from TimeSlot_List
                    Date = _context.TimeSlot_List
                        .Where(ts => ts.TimeSlotId == b.TimeSlotId)
                        .Select(ts => ts.SelectedDates)
                        .FirstOrDefault(), // Assuming SelectedDates is the property name
                    TimeSlotId = b.TimeSlotId
                })
                .FirstOrDefaultAsync();

            // Fetch the time slot details for the current booking if it exists
            string currentTimeSlot = null;
            if (currentBooking != null)
            {
                currentTimeSlot = await _context.TimeSlot_List
                    .Where(ts => ts.TimeSlotId == currentBooking.TimeSlotId)
                    .Select(ts => ts.TimeSlots)
                    .FirstOrDefaultAsync();
            }

            // Debugging: Print available slots to the console
            if (availableSlots.Count == 0)
            {
                Console.WriteLine("No available slots found.");
            }
            else
            {
                foreach (var group in availableSlots)
                {
                    Console.WriteLine($"Date: {group.date}");
                    foreach (var slot in group.timeSlots)
                    {
                        Console.WriteLine($"TimeSlotId: {slot.TimeSlotId}, TimeSlots: {slot.TimeSlots}");
                    }
                }
            }

            // Return both available slots and current booking time slot (if any)
            return Json(new
            {
                AvailableSlots = availableSlots,
                CurrentBooking = currentBooking != null ? new
                {
                    Date = currentBooking.Date,
                    TimeSlotId = currentBooking.TimeSlotId,
                    TimeSlots = currentTimeSlot // Include the time slot details
                } : null
            });
        }




        // Define a class to deserialize the request
        public class WorkerRequest
        {
            public int WorkerId { get; set; }
            public int ServiceId { get; set; }
            public int TimeSlotId { get; set; }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worker_Id,BookingDate,Service_Id,AgreeToTerms,TimeSlotId")] Booking booking)
        {
            // Print all relevant data to the console
            Console.WriteLine($"Worker ID: {booking.Worker_Id}");
            Console.WriteLine($"Service ID: {booking.Service_Id}");
            Console.WriteLine($"Booking Date: {booking.BookingDate}");
            Console.WriteLine($"Agree to Terms: {booking.AgreeToTerms}");
            Console.WriteLine($"Time Slot ID: {booking.TimeSlotId}");

            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // This gets the logged-in user's ID
            Console.WriteLine($"User ID: {userId}");

            // Assign the UserId to the booking
            booking.UserId = userId;

            if (ModelState.IsValid)
            {
                // Find the time slot in the database using the provided TimeSlotId
                var timeSlot = await _context.TimeSlot_List.FindAsync(booking.TimeSlotId);

                if (timeSlot != null)
                {
                    timeSlot.IsBooked = true;
                    _context.Update(timeSlot); // Update the time slot in the context
                }
                else
                {
                    ModelState.AddModelError("TimeSlotId", "Selected time slot is not valid.");
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Paypal", new { workerId = booking.Worker_Id });

            }

            return View(booking);
        }




        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Set the TimeSlotId from the booking entity
            ViewData["TimeSlotId"] = booking.TimeSlotId; // Assuming TimeSlotId is a property of Booking
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Service_Id,Worker_Id,BookingDate,AgreeToTerms")] Booking booking, int TimeSlotId)
        {
            // Log the incoming TimeSlotId for debugging
            Console.WriteLine($"Editing Booking ID: {id}");
            Console.WriteLine($"Incoming TimeSlotId: {TimeSlotId}");

            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the previous booking
                    var previousBooking = await _context.Booking.FindAsync(id);
                    if (previousBooking != null)
                    {
                        // Log previous booking details for debugging
                        Console.WriteLine($"Previous Booking Details - Worker ID: {previousBooking.Worker_Id}, TimeSlotId: {previousBooking.TimeSlotId}");

                        // Mark the previously booked slot as not booked
                        var previousTimeSlot = await _context.TimeSlot_List.FindAsync(previousBooking.TimeSlotId);
                        if (previousTimeSlot != null)
                        {
                            previousTimeSlot.IsBooked = false; // Unmark the old time slot
                            Console.WriteLine($"Unmarking Previous TimeSlotId: {previousBooking.TimeSlotId}");
                        }

                        // Check if the new TimeSlotId exists
                        var newTimeSlot = await _context.TimeSlot_List.FindAsync(TimeSlotId);
                        if (newTimeSlot != null)
                        {
                            newTimeSlot.IsBooked = true; // Mark the new time slot as booked
                            Console.WriteLine($"Marking New TimeSlotId: {TimeSlotId} as booked");
                        }
                        else
                        {
                            ModelState.AddModelError("TimeSlotId", "The selected time slot does not exist.");
                            return View(booking);
                        }

                        // Update the booking information
                        previousBooking.Service_Id = booking.Service_Id;
                        previousBooking.Worker_Id = booking.Worker_Id;
                        previousBooking.BookingDate = booking.BookingDate;
                        previousBooking.AgreeToTerms = booking.AgreeToTerms;
                        previousBooking.TimeSlotId = TimeSlotId; // Update the TimeSlotId for the booking

                        // Save changes to the context
                        _context.Update(previousBooking);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
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