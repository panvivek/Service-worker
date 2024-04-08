using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: TimeSlots/Create
        // GET: TimeSlots/Create
        public IActionResult Create()
        {
            // Retrieve the last worker's id from the database
            var lastWorkerId = _context.Worker_List
                                        .OrderByDescending(w => w.Worker_Id)
                                        .Select(w => w.Worker_Id)
                                        .FirstOrDefault();

            var workers = _context.Worker_List
                            .Select(w => new SelectListItem
                            {
                                Value = w.Worker_Id.ToString(),
                                Text = w.Name
                            })
                            .ToList();

            // Pass the list of workers to the view
            ViewBag.Workers = workers;

            // Pass the last worker's id to the view
            ViewBag.LastWorkerId = lastWorkerId;

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeSlotId,StartTime,EndTime,IsBooked")] TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the last worker's ID from the database
                var lastWorkerId = _context.Worker_List
                                          .OrderByDescending(w => w.Worker_Id)
                                          .Select(w => w.Worker_Id)
                                          .FirstOrDefault();

                // Assign the last worker's ID to the TimeSlot object
                timeSlot.Worker_Id = lastWorkerId;

                // Add the TimeSlot object to the context and save changes
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();

                // Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the TimeSlot object
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
