﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
       
        public async Task<IActionResult> Index(int serviceId, string sortOrder)
        {
            ViewData["ServiceId"] = serviceId;
            ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewData["RatingSortParam"] = sortOrder == "ratings_asc" ? "ratings_desc" : "ratings_asc";

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
               
                default:
                    workers = workers.OrderBy(w => w.Price);
                    break;
            }

            return View(workers);
        }

        public async Task<IActionResult> Dashboard(int? id)
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

            return View(worker.Price);
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
        // GET: Workers/Create
        public async Task<IActionResult> Create(string userId) // Make userId optional
        {
            // Prepare data for the view (e.g., service list) even if userId is null
            var services = await _context.Services_List.ToListAsync();
            var serviceItems = services.Select(s => new SelectListItem
            {
                Value = s.Service_Id.ToString(),
                Text = s.Name
            }).ToList();

            ViewBag.Services = serviceItems;

            var worker = new Worker();

            
                worker.UserId = userId; // Pre-populate UserId if available
            

            return View(worker);
        }

        // POST: Workers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Worker worker, int[] Service_Id)
        {
            if (ModelState.IsValid)
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name == "Worker")
                    .Select(r => r.Id)
                .FirstOrDefaultAsync();


                

                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);





                worker.RoleId = roleId;
                worker.UserId = user;



                if (string.IsNullOrEmpty(worker.UserId))
                {
                    return BadRequest("UserId is required."); // Or handle appropriately
                }

                _context.Add(worker);
                await _context.SaveChangesAsync(); // Save the worker first to get the generated Worker_Id

                // Insert selected services into WorkerService table
                if (Service_Id != null && Service_Id.Length > 0)
                {
                    foreach (var serviceId in Service_Id)
                    {
                        var workerService = new WorkerService
                        {
                            Worker_Id = worker.Worker_Id, // Use the generated Worker_Id
                            Service_Id = serviceId
                        };

                        _context.Add(workerService);
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["WorkerId"] = worker.Worker_Id;
                return RedirectToAction("Create", "TimeSlots");
            }

            // If ModelState is not valid, repopulate the services list and return to the view
            var services = await _context.Services_List.ToListAsync();
            var serviceItems = services.Select(s => new SelectListItem
            {
                Value = s.Service_Id.ToString(),
                Text = s.Name
            }).ToList();

            ViewBag.Services = serviceItems;
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
