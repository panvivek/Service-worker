using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Areas.Identity.Data;
using ServiceWorkerWebsite.Data;

namespace ServiceWorkerWebsite.Controllers
{
    public class WorkerServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;

        public WorkerServicesController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: WorkerServices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkerServices.Include(w => w.Service).Include(w => w.Worker);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: WorkerServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WorkerServices == null)
            {
                return NotFound();
            }

            var workerService = await _context.WorkerServices
                .Include(w => w.Service)
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(m => m.Worker_Id == id);
            if (workerService == null)
            {
                return NotFound();
            }

            return View(workerService);
        }

        // GET: WorkerServices/Create
        public IActionResult Create()
        {
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id");
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id");
            return View();
        }

        // POST: WorkerServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkerServiceId,Worker_Id,Service_Id")] WorkerService workerService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workerService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", workerService.Service_Id);
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id", workerService.Worker_Id);
            return View(workerService);
        }

        // GET: WorkerServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WorkerServices == null)
            {
                return NotFound();
            }

            var workerService = await _context.WorkerServices.FindAsync(id);
            if (workerService == null)
            {
                return NotFound();
            }
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", workerService.Service_Id);
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id", workerService.Worker_Id);
            return View(workerService);
        }

        // POST: WorkerServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkerServiceId,Worker_Id,Service_Id")] WorkerService workerService)
        {
            if (id != workerService.Worker_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workerService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerServiceExists(workerService.Worker_Id))
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
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", workerService.Service_Id);
            ViewData["Worker_Id"] = new SelectList(_context.Worker_List, "Worker_Id", "Worker_Id", workerService.Worker_Id);
            return View(workerService);
        }

        // GET: WorkerServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WorkerServices == null)
            {
                return NotFound();
            }

            var workerService = await _context.WorkerServices
                .Include(w => w.Service)
                .Include(w => w.Worker)
                .FirstOrDefaultAsync(m => m.Worker_Id == id);
            if (workerService == null)
            {
                return NotFound();
            }

            return View(workerService);
        }

        // POST: WorkerServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WorkerServices == null)
            {
                return Problem("Entity set 'ApplicationDbContext.WorkerServices'  is null.");
            }
            var workerService = await _context.WorkerServices.FindAsync(id);
            if (workerService != null)
            {
                _context.WorkerServices.Remove(workerService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerServiceExists(int id)
        {
            return _context.WorkerServices.Any(e => e.Worker_Id == id);
        }
    }
}