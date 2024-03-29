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
        public IActionResult Create()
        {
            ViewData["Worke_Id"] = new SelectList(_context.Worker_List, "Worke_Id", "Worke_Id");
            return View();
        }

        // POST: TimeSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeSlotId,StartTime,EndTime,IsBooked,Worke_Id")] TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Worke_Id"] = new SelectList(_context.Worker_List, "Worke_Id", "Worke_Id", timeSlot.Worke_Id);
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
            ViewData["Worke_Id"] = new SelectList(_context.Worker_List, "Worke_Id", "Worke_Id", timeSlot.Worke_Id);
            return View(timeSlot);
        }

        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimeSlotId,StartTime,EndTime,IsBooked,Worke_Id")] TimeSlot timeSlot)
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
            ViewData["Worke_Id"] = new SelectList(_context.Worker_List, "Worke_Id", "Worke_Id", timeSlot.Worke_Id);
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
                return Problem("Entity set 'ApplicationDbContext.TimeSlots'  is null.");
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
          return (_context.TimeSlot_List   ?.Any(e => e.TimeSlotId == id)).GetValueOrDefault();
        }
    }
}
