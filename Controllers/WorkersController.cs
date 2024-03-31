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
    public class WorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Workers
        /*public async Task<IActionResult> Index(string speciality)
        {
            if (!string.IsNullOrEmpty(speciality))
            {
                // Filter workers by speciality (assuming Name is the speciality)
                var workers = await _context.Worker_List
                    .Where(w => w.Speciality == speciality)
                    .ToListAsync();

                return View(workers);
            }
            else
            {
                // If no speciality is provided, return all workers
                return View(await _context.Worker_List.ToListAsync());
            }
        } */

        public async Task<IActionResult> Index(string speciality)
        {
            // Query to fetch workers, optionally filtered by speciality
            var workersQuery = _context.Worker_List.AsQueryable();

            if (!string.IsNullOrEmpty(speciality))
            {
                // Filter by speciality
                workersQuery = workersQuery.Where(w => w.Speciality == speciality);
            }

            // Include AvailableTimeSlots navigation property
            var workersWithSlots = workersQuery.Select(worker => new
            {
                Worker = worker,
                AvailableTimeSlots = _context.TimeSlot_List.Where(ts => ts.Worke_Id == worker.Worke_Id && !ts.IsBooked).ToList()
            }).ToListAsync();

            // Map the result to your ViewModel or directly to your view, if applicable
            var workers = (await workersWithSlots).Select(ws => new Worker
            {
                Worke_Id = ws.Worker.Worke_Id,
                Name = ws.Worker.Name,
                Speciality = ws.Worker.Speciality,
                Availability_Status = ws.Worker.Availability_Status,
                Ratings = ws.Worker.Ratings,
                Reviews = ws.Worker.Reviews,

                // Other properties...
                AvailableTimeSlots = ws.AvailableTimeSlots // Assuming your ViewModel has a property for AvailableTimeSlots
            }).ToList();

            return View(workers);
        }




        // GET: Workers
        /* public async Task<IActionResult> Index()
         {
               return _context.Worker_List != null ? 
                           View(await _context.Worker_List.ToListAsync()) :
                           Problem("Entity set 'ApplicationDbContext.Worker_List'  is null.");
         }
        */

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Worker_List == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker_List
                .FirstOrDefaultAsync(m => m.Worke_Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worke_Id,Name,Speciality,Availability_Status,Ratings,Reviews")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Worker_List == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker_List.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Worke_Id,Name,Speciality,Availability_Status,Ratings,Reviews")] Worker worker)
        {
            if (id != worker.Worke_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Worke_Id))
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
            return View(worker);
        }

        // GET: Workers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Worker_List == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker_List
                .FirstOrDefaultAsync(m => m.Worke_Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Worker_List == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Worker_List'  is null.");
            }
            var worker = await _context.Worker_List.FindAsync(id);
            if (worker != null)
            {
                _context.Worker_List.Remove(worker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
          return (_context.Worker_List?.Any(e => e.Worke_Id == id)).GetValueOrDefault();
        }
    }
}
