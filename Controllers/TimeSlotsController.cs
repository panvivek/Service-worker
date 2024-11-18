using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;

namespace ServiceWorkerWebsite.Controllers
{
    public class TimeSlotsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimeSlotsController> _logger;

        public TimeSlotsController(ApplicationDbContext context, ILogger<TimeSlotsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Worker,Admin")]
        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get the logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"Current UserId: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No user ID found");
                    return RedirectToAction("Login", "Account");
                }

                // Get worker details
                var worker = await _context.Worker_List
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (worker == null)
                {
                    _logger.LogWarning($"No worker found for userId: {userId}");
                    return RedirectToAction("Create", "Workers");
                }

                _logger.LogInformation($"Found Worker_Id: {worker.Worker_Id}");

                // Get time slots
                var timeSlots = await _context.TimeSlot_List
                    .Where(t => t.Worker_Id == worker.Worker_Id)
                    .OrderBy(t => t.SelectedDates)
                    .ThenBy(t => t.TimeSlots)
                    .ToListAsync();

                _logger.LogInformation($"Found {timeSlots.Count} time slots");

                // Log each time slot for debugging
                foreach (var slot in timeSlots)
                {
                    _logger.LogInformation(
                        $"TimeSlot: ID={slot.TimeSlotId}, " +
                        $"Date={slot.SelectedDates}, " +
                        $"Time={slot.TimeSlots}, " +
                        $"IsBooked={slot.IsBooked}"
                    );
                }

                ViewBag.WorkerId = worker.Worker_Id;
                ViewBag.UserEmail = User.Identity.Name;

                return View(timeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Index: {ex.Message}");
                throw;
            }
        }

        // GET: TimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TimeSlot_List == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot_List
                .Include(t => t.Worker)
                .FirstOrDefaultAsync(m => m.TimeSlotId == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }
        [Authorize(Roles = "Worker,Admin")]

        // GET: TimeSlots/Create
        // GET: TimeSlots/Create
        public IActionResult Create(int workerId)
        {
            ViewBag.Worker_Id = workerId;

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worker_Id,SelectedDates,TimePeriod,TimeSlots")] TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                // Parse SelectedDates
                var selectedDatesList = Request.Form["SelectedDates"]
                    .ToString()
                    .Split(',')
                    .Select(d => DateTime.Parse(d.Trim()))
                    .ToList();

                // Parse TimeSlots
                var selectedTimeSlots = Request.Form["TimeSlots"].ToArray();

                // Loop through each selected date
                foreach (var date in selectedDatesList)
                {
                    // Create a new TimeSlot entry for each time slot on this date
                    foreach (var slot in selectedTimeSlots)
                    {
                        var newTimeSlot = new TimeSlot
                        {
                            Worker_Id = timeSlot.Worker_Id,
                            SelectedDates = date.ToString("yyyy-MM-dd"), // Store only the current date
                                                                         // If you want to keep this
                            TimeSlots = slot, // Store the specific time slot
                            IsBooked = false // Default to not booked
                        };

                        // Save the new time slot to the context
                        _context.TimeSlot_List.Add(newTimeSlot);
                    }
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(timeSlot);
        }





        // GET: TimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TimeSlot_List == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot_List.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id", timeSlot.Worker_Id);
            return View(timeSlot);
        }

        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimeSlotId,StartTime,EndTime,IsBooked,Worker_Id")] TimeSlot timeSlot)
        {
            if (id != timeSlot.TimeSlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeSlotExists(timeSlot.TimeSlotId))
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
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id", timeSlot.Worker_Id);
            return View(timeSlot);
        }

        // GET: TimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TimeSlot_List == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot_List
                .Include(t => t.Worker)
                .FirstOrDefaultAsync(m => m.TimeSlotId == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingDetails(int timeSlotId)
        {
            var booking = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.TimeSlotId == 118);

            if (booking == null)
            {
                // Return the partial view with null model to show "no booking" message
                Console.WriteLine("No booking found for timeSlotId: " + timeSlotId);
                return PartialView("_BookingDetails", null);
            }

            return PartialView("_BookingDetails", booking);
        }


        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TimeSlot_List == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TimeSlot_List'  is null.");
            }
            var timeSlot = await _context.TimeSlot_List.FindAsync(id);
            if (timeSlot != null)
            {
                _context.TimeSlot_List.Remove(timeSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeSlotExists(int id)
        {
            return _context.TimeSlot_List.Any(e => e.TimeSlotId == id);
        }


    }
}
