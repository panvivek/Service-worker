using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Areas.Identity.Data;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System.Security.Claims;

namespace ServiceWorkerWebsite.Controllers
{
    [Authorize(Roles = "Worker")]
    public class BusinessController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;

        public BusinessController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Earnings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var worker = await _context.Worker_List
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (worker == null)
            {
                return NotFound("Worker profile not found");
            }

            var workerServices = await _context.WorkerServices
                .Include(ws => ws.Service)
                .Where(ws => ws.Worker_Id == worker.Worker_Id)
                .Select(ws => ws.Service)
                .ToListAsync();

            var bookings = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.TimeSlot)
                .Include(b => b.Worker)
                    .ThenInclude(w => w.User)
                    .Include(b => b.User)
                .Where(b => b.Worker_Id == worker.Worker_Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var serviceEarnings = bookings
                .GroupBy(b => b.Service.Name)
                .Select(g => new ServiceEarningsViewModel
                {
                    ServiceName = g.Key,
                    TotalBookings = g.Count(),
                    TotalEarnings = g.Sum(b => worker.Price)
                })
                .ToList();

            var monthlyEarnings = bookings
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new MonthlyEarningsViewModel
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalEarnings = g.Sum(b => worker.Price),
                    NumberOfBookings = g.Count()
                })
                .OrderByDescending(m => m.Month)
                .ToList();

            var viewModel = new BusinessEarningsViewModel
            {
                Worker = worker,
                Services = workerServices,
                Bookings = bookings,
                ServiceEarnings = serviceEarnings,
                MonthlyEarnings = monthlyEarnings,
                TotalEarnings = bookings.Sum(b => worker.Price),
                UniqueCustomers = bookings.Select(b => b.UserId).Distinct().Count()
            };

            return View(viewModel);
        }
    }
}