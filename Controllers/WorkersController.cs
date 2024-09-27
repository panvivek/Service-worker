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
    public class WorkersController : Controller
    {

        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: Workers
        public async Task<IActionResult> Index(int serviceId, string sortOrder, double? latitude = 0.0, double? longitude = 0.0)
        {
            //string message = Session[]
            ViewData["ServiceId"] = serviceId;
            ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewData["RatingSortParam"] = sortOrder == "ratings_asc" ? "ratings_desc" : "ratings_asc";
            //ViewData["LocationSortParam"] = "location";

            var serviceWithWorkers = await _context.Services_List
                .Include(s => s.WorkerServices)
                .ThenInclude(ws => ws.Worker)
                .FirstOrDefaultAsync(s => s.Service_Id == serviceId);

            if (serviceWithWorkers == null)
            {
                return NotFound();
            }

            // Extract the workers associated with the service
            var workers = serviceWithWorkers.WorkerServices.Select(ws => ws.Worker);

            // Sorting logic
            switch (sortOrder)
            {
                case "price_desc":
                    workers = workers.OrderByDescending(w => w.Price);
                    break;
                case "ratings_asc":
                    workers = workers.OrderBy(w => w.Ratings);
                    break;
                case "ratings_desc":
                    workers = workers.OrderByDescending(w => w.Ratings);
                    break;
                //case "location":
                //    workers = workers.Where(w => (w.Latitude <= (latitude + 2) && w.Latitude >= (latitude - 2)) && (w.Longitude <= (longitude + 2) && w.Longitude >= (longitude - 2)));
                //    //workers = workers.
                //    break;
                default:
                    workers = workers.OrderBy(w => w.Price);
                    break;
            }

            return View(workers);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLocation(int serviceId, double latitude, double longitude)
        {
            // Now you can work with the latitude and longitude values
            // Example: Save them to a database, or use them in further logic.
            ViewData["ServiceId"] = serviceId;
            ViewData["LocationSortParam"] = "location";

            var serviceWithWorkers = await _context.Services_List
                .Include(s => s.WorkerServices)
                .ThenInclude(ws => ws.Worker)
                .FirstOrDefaultAsync(s => s.Service_Id == serviceId);

            if (serviceWithWorkers == null)
            {
                return NotFound();
            }

            // Extract the workers associated with the service
            var workers = serviceWithWorkers.WorkerServices.Select(ws => ws.Worker);

            //workers = workers.OrderBy(w => (w.Latitude >= (latitude + 2) && w.Latitude <= (latitude - 2)) && (w.Longitude >= (longitude + 2) && w.Longitude <= (longitude - 2)));

            return View(workers);
        }

        // GET: Workers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Worker_List == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker_List
                .FirstOrDefaultAsync(m => m.Worker_Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            var services = _context.Services_List.ToList();
            var serviceItems = services.Select(s => new SelectListItem
            {
                Value = s.Service_Id.ToString(),
                Text = s.Name
            }).ToList();




            ViewBag.Services = serviceItems;

            return View();
        }




        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // POST: Workers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Worker worker, int[] Service_Id)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worker);
                await _context.SaveChangesAsync();

                // Insert selected services into WorkerService table
                if (Service_Id != null && Service_Id.Length > 0)
                {
                    foreach (var serviceId in Service_Id)
                    {
                        // Create a new WorkerService object and set its properties
                        var workerService = new WorkerService
                        {
                            Worker_Id = worker.Worker_Id,
                            Service_Id = serviceId
                        };

                        // Add the new WorkerService object to the context
                        _context.Add(workerService);
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    TempData["WorkerId"] = worker.Worker_Id;
                    return RedirectToAction("Create", "TimeSlots");


                }

                // Redirect to the Index action
                // return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return the Create view with the worker model
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
        public async Task<IActionResult> Edit(int id, [Bind("Worker_Id,ProfilePic_Id,Name,Availability_Status,Ratings,Reviews,Price")] Worker worker)
        {
            if (id != worker.Worker_Id)
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
                    if (!WorkerExists(worker.Worker_Id))
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
                .FirstOrDefaultAsync(m => m.Worker_Id == id);
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
            return _context.Worker_List.Any(e => e.Worker_Id == id);
        }
    }
}