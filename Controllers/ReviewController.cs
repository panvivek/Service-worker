using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServiceWorkerWebsite.Controllers
{
    public class ReviewController : Controller


    {




        private readonly ApplicationDbContext _context;
        public ReviewController(ApplicationDbContext context)
        {
            _context = context;

        }


        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return View(reviews);
        }

        // GET: Reviews/Create
        public async Task<IActionResult> Create(int workerId, int serviceId, string userId)
        {
            // Optionally, fetch a list of workers to populate a dropdown in the view
            // ViewBag.WorkerList = await _context.Worker_List.ToListAsync(); 

            var customer = await _context.Users.FindAsync(userId); // Assuming 'Users' is your Identity table

            if (customer == null)
            {
                return NotFound(); // Handle case where the user doesn't exist
            }

            // Pass workerId and customer name to the view
            ViewBag.WorkerId = workerId;
            ViewBag.ServiceId = serviceId;
            ViewBag.CustomerName = customer.Firstname + " " + customer.Lastname; // Combine first and last name


            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worker_Id,Service_Id, RatingValue, Comment,CustomerName")] Reviews review)
        {
            if (ModelState.IsValid)
            {

                ViewBag.WorkerId = review.Worker_Id;
                review.ReviewDate = DateTime.Now;
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details",
 "Workers", new { Id = review.Worker_Id, serviceId = review.Service_Id }); // Redirect to worker details
            }
            // Optionally, repopulate the worker list if needed
            // ViewBag.WorkerList = await _context.Worker_List.ToListAsync();
            return View(review);
        }

        // ... (Other actions like Edit, Delete if needed) 
    }
}
