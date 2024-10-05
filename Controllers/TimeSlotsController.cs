using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public TimeSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Worker,Admin")]
        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TimeSlot_List.Include(t => t.Worker);
            return View(await applicationDbContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewBag.Workers = new SelectList(
                _context.Worker_List.Select(w => new { w.Worker_Id }),
                "Worker_Id",
                "Worker_Id"  // Use Worker_Id as both the value and the display text
            );

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
                            TimePeriod = timeSlot.TimePeriod, // If you want to keep this
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
