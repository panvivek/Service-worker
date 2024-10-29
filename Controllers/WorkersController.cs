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
    public class WorkersController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;

        public WorkersController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int serviceId, string sortOrder)
        {
            ViewData["ServiceId"] = serviceId;
            ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewData["RatingSortParam"] = sortOrder == "ratings_asc" ? "ratings_desc" : "ratings_asc";
            ViewData["LocationParam"] = sortOrder == "locationAsc" ? "locationdesc" : "locationAsc";

            var serviceWithWorkers = await _context.Services_List
                .Include(s => s.WorkerServices)
                .ThenInclude(ws => ws.Worker)
                .ThenInclude(w => w.User)
                .FirstOrDefaultAsync(s => s.Service_Id == serviceId);

            if (serviceWithWorkers == null)
            {
                return NotFound();
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Or however you retrieve the user ID
            var userAddress = await _context.UserAddress
                .FirstOrDefaultAsync(a => a.UserId == loggedInUserId);
            //string userCity = userAddress.City;

            // Extract the workers associated with the service
            //var workers = serviceWithWorkers.WorkerServices.Select(ws => ws.Worker);

            // Join with UserAddress to get worker address details
            //var workersWithAddress = from worker in workers
            //                         join address in _context.UserAddress
            //                         on worker.UserId equals address.UserId
            //                         select new
            //                         {
            //                             Worker = worker,
            //                             Address = address,
            //                         };

            // Query to display the name of the worker
            var workersQuery = serviceWithWorkers.WorkerServices
                .Select(ws => new
                {
                    WorkerId = ws.Worker.Worker_Id,
                    UserId = ws.Worker.UserId,
                    Price = ws.Worker.Price,
                    ProfilePic_Id = ws.Worker.ProfilePic_Id,
                    FirstName = ws.Worker.User.Firstname,
                    LastName = ws.Worker.User.Lastname
                });


            // Sorting logic
            switch (sortOrder)
            {
                case "price_desc":
                    workersQuery = workersQuery.OrderByDescending(w => w.Price);
                    break;

                // Location Filter Added
                case "locationAsc":
                    //    var filteredWorkers = workersWithAddress
                    //        .Where(w => w.Address.City.Equals(userAddress.City, StringComparison.OrdinalIgnoreCase));
                    //    workers = filteredWorkers.Select(w => w.Worker);  // Extract only workers
                    //    break;
                    var workersWithAddress = from w in workersQuery
                                             join addr in _context.UserAddress
                                             on w.UserId equals addr.UserId
                                             where addr.City.Equals(userAddress.City,
                                                   StringComparison.OrdinalIgnoreCase)
                                             select w;
                    workersQuery = workersWithAddress;
                    break;

                default:
                    workersQuery = workersQuery.OrderBy(w => w.Price);
                    break;
            }

            var workers = workersQuery.ToList();
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


        public async Task<IActionResult> Manage()
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            // Fetch the worker associated with the logged-in user
            var worker = await _context.Worker_List
                .Include(w => w.WorkerServices)
                    .ThenInclude(ws => ws.Service) // Include related services
                .FirstOrDefaultAsync(w => w.UserId == currentUser.Id);

            if (worker == null)
            {
                return NotFound("Worker not found.");
            }

            // Pass worker and their services to the view
            return View(worker);
        }

        // GET: Workers/Details/5

        public async Task<IActionResult> Details(int workerId, int serviceId, int page = 1)
        {
            int pageSize = 5; // Number of reviews per page

            // Fetch the worker along with the reviews for the specific service
            var worker = await _context.Worker_List
                .Include(w => w.Reviews.Where(r => r.Service_Id == serviceId)) // Filter reviews by service
                .FirstOrDefaultAsync(w => w.Worker_Id == workerId);

            if (worker == null)
            {
                return NotFound();
            }

            // Paginate the reviews
            var totalReviews = worker.Reviews.Count();
            var reviewsToShow = worker.Reviews
                .OrderBy(r => r.ReviewDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewViewModel
                {
                    RatingValue = r.RatingValue,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate,
                    CustomerName = string.IsNullOrEmpty(r.CustomerName) ? "Anonymous" : r.CustomerName
                })
                .ToList();

            // Calculate average rating for the service
            double averageRating = worker.Reviews.Any() ? worker.Reviews.Average(r => r.RatingValue) : 0;

            // Pass data to the view
            ViewBag.AverageRating = averageRating;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalReviews / pageSize);
            ViewBag.ServiceId = serviceId; // To return back to the correct service

            var workerDetailsViewModel = new WorkerDetailsViewModel
            {
                Worker_Id = worker.Worker_Id,
                ProfilePicUrl = worker.ProfilePic_Id,
                Price = worker.Price,
                Reviews = reviewsToShow // Paginated reviews
            };

            return View(workerDetailsViewModel);
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
        public async Task<IActionResult> Create(Worker worker, int[] Service_Id, IFormFile ProfilePicFile, string CapturedImage)
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


                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "WorkerProfilePic");
                if (string.IsNullOrEmpty(worker.UserId))
                {
                    return BadRequest("UserId is required."); // Or handle appropriately
                }



                if (ProfilePicFile != null && ProfilePicFile.Length > 0)
                {

                    

                   

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        Console.WriteLine($"Created directory: {uploadsFolder}");
                    }

                    // Generate a unique file name
                   

                    var uniqueFileName = $"{ProfilePicFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    Console.WriteLine($"Saving file to: {filePath}");

                    // Save the file to the specified path
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicFile.CopyToAsync(fileStream);
                    }

                    // Store the relative path in the database
                    worker.ProfilePic_Id = $"/WorkerProfilePic/{uniqueFileName}";
                }
                else if (!string.IsNullOrEmpty(CapturedImage))
                {

                    // Save the captured image from the webcam
                    var base64Data = CapturedImage.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);

                    string uniqueFileName = $"{Guid.NewGuid()}.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    worker.ProfilePic_Id = $"/WorkerProfilePic/{uniqueFileName}";
                }
                else
                {
                    // Handle case where no file is uploaded (Optional)
                    worker.ProfilePic_Id = "/images/default-profile.png";
                    Console.WriteLine("No file uploaded. Using default profile picture.");
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
                return RedirectToAction("Create", "TimeSlots", new { workerId = worker.Worker_Id });
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
        public async Task<IActionResult> Edit()
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            // Fetch the worker associated with the logged-in user along with services
            var worker = await _context.Worker_List
                .Include(w => w.WorkerServices)
                    .ThenInclude(ws => ws.Service) // Include related services
                .FirstOrDefaultAsync(w => w.UserId == currentUser.Id);

            if (worker == null)
            {
                return NotFound("Worker not found.");
            }

            // Fetch all available services for the dropdown
            var workerServiceItems = worker.WorkerServices.Select(ws => new SelectListItem
            {
                Value = ws.Service.Service_Id.ToString(),
                Text = ws.Service.Name,
                Selected = true // Mark them as selected (if required for form display)
            }).ToList();

            ViewBag.Services = workerServiceItems; // Pass services to the view

            return View(worker);
        }


        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Worker_Id,ProfilePic_Id,Name,Availability_Status,Ratings,Reviews,Price")] Worker worker, int[] ServiceIds, IFormFile ProfilePicFile, string CapturedImage)
        {
            if (!ModelState.IsValid)
            {
                // Reload services for dropdown in case of validation failure
                var allServices = await _context.Services_List.ToListAsync();
                ViewBag.Services = allServices.Select(s => new SelectListItem
                {
                    Value = s.Service_Id.ToString(),
                    Text = s.Name,
                    Selected = ServiceIds.Contains(s.Service_Id)
                }).ToList();
                return View(worker);
            }

            // Fetch the existing worker record
            var existingWorker = await _context.Worker_List
                .Include(w => w.WorkerServices)
                .FirstOrDefaultAsync(w => w.Worker_Id == worker.Worker_Id);

            if (existingWorker == null)
            {
                return NotFound("Worker not found.");
            }

            try
            {

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "WorkerProfilePic");

                if (ProfilePicFile != null && ProfilePicFile.Length > 0)
                {
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}_{ProfilePicFile.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicFile.CopyToAsync(fileStream);
                    }

                    existingWorker.ProfilePic_Id = $"/WorkerProfilePic/{uniqueFileName}";
                }
                else if (!string.IsNullOrEmpty(CapturedImage))
                {
                    // Handle captured webcam image
                    var base64Data = CapturedImage.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);

                    string uniqueFileName = $"{Guid.NewGuid()}.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    existingWorker.ProfilePic_Id = $"/WorkerProfilePic/{uniqueFileName}";
                }
                
                

                    existingWorker.ProfilePic_Id = worker.ProfilePic_Id;
                    existingWorker.Price = worker.Price;

                // Remove old services not in the new selection
                _context.WorkerServices.RemoveRange(
                    existingWorker.WorkerServices.Where(ws => !ServiceIds.Contains(ws.Service_Id))
                );

                // Add new services selected by the worker
                foreach (var serviceId in ServiceIds)
                {
                    if (!existingWorker.WorkerServices.Any(ws => ws.Service_Id == serviceId))
                    {
                        _context.WorkerServices.Add(new WorkerService
                        {
                            Worker_Id = existingWorker.Worker_Id,
                            Service_Id = serviceId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Worker details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating worker: {ex.Message}";
                return View(worker);
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker)
             
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
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
