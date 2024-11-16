using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceWorkerWebsite.Controllers
{

    public class WorkerEarningsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkerEarningsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet("{workerId}")]
        [HttpGet]
        public async Task<IActionResult> Index(int workerId)
        {
            // Fetch the worker and related data
            var worker = await _context.Worker_List
                .Include(w => w.WorkerServices)
                    .ThenInclude(ws => ws.Service)
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.Worker_Id == workerId);

            if (worker == null)
                return NotFound();

            // Fetch bookings for the worker
            var bookings = await _context.Booking
                .Include(b => b.Service)
                .Where(b => b.Worker_Id == workerId)
                .ToListAsync();

            // Prepare earnings by service
            //var serviceEarnings = worker.WorkerServices
            //    .GroupJoin(bookings,
            //        ws => ws.Service_Id,
            //        b => b.Service_Id,
            //        (ws, serviceBookings) => new ServiceEarningsViewModel
            //        {
            //            ServiceName = ws.Service.Name,
            //            BasePrice = worker.Price,
            //            TotalBookings = serviceBookings.Count(),
            //            TotalEarnings = serviceBookings.Count() * worker.Price
            //        })
            //    .ToList();

            var serviceEarnings = worker.WorkerServices
                .Select(ws => new ServiceEarningsViewModel
                {
                    ServiceName = ws.Service.Name,
                    BasePrice = worker.Price,
                    TotalBookings = bookings.Count(b => b.Service_Id == ws.Service_Id),
                    TotalEarnings = bookings.Count(b => b.Service_Id == ws.Service_Id) * worker.Price
                })
                .ToList();

            // Prepare the view model
            var viewModel = new WorkerEarningsViewModel
            {
                WorkerId = workerId,
                TotalEarnings = serviceEarnings.Sum(s => s.TotalEarnings),
                TotalBookings = bookings.Count,
                ServicesCount = worker.WorkerServices.Count,
                ServiceEarnings = serviceEarnings,
                //WorkerName = $"{worker.User.Firstname} {worker.User.Lastname}"
            };

            // Return the view with the view model
            return View(viewModel);
        }
    }
}