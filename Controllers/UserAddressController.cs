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
using ServiceWorkerWebsite.Models;

namespace ServiceWorkerWebsite.Controllers
{
    public class UserAddressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;

        public UserAddressController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserAddress
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.UserAddress.Include(u => u.User);
            //return View(await applicationDbContext.ToListAsync());
            var userId = _userManager.GetUserId(User);

            var userAddresses = await _context.UserAddress
                                               .Where(ua => ua.UserId == userId)
                                               .ToListAsync();

            return View(userAddresses);
        }

        // GET: UserAddress/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddress
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserAdd_Id == id);
            if (userAddress == null)
            {
                return NotFound();
            }

            return View(userAddress);
        }

        // GET: UserAddress/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserAddress/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserAdd_Id,UserId,StreetNumberName,PostalCode,City,Province,Country")] UserAddress userAddress)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user's ID
                var userId = _userManager.GetUserId(User);

                // Set the UserId for the address
                userAddress.UserId = userId;

                // Add the new UserAddress to the context
                _context.Add(userAddress);
                await _context.SaveChangesAsync();

                // Retrieve the user's role
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                // Check if the user is in the 'Worker' role
                if (await _userManager.IsInRoleAsync(user, "Worker"))
                {
                    return RedirectToAction("Create", "Workers");
                }
                // Check if the user is in the 'Customer' role
                else if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("Index", "Home");
                }

                //return RedirectToAction("Index", "Home"); // Redirect to the index page or any other page you prefer
            }
            return View(userAddress);
        }

        // GET: UserAddress/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddress.FindAsync(id);
            if (userAddress == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userAddress.UserId);
            return View(userAddress);
        }

        // POST: UserAddress/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserAdd_Id,UserId,StreetNumberName,PostalCode,City,Province,Country")] UserAddress userAddress)
        {
            if (id != userAddress.UserAdd_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAddressExists(userAddress.UserAdd_Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userAddress.UserId);
            return View(userAddress);
        }

        // GET: UserAddress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAddress = await _context.UserAddress
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserAdd_Id == id);
            if (userAddress == null)
            {
                return NotFound();
            }

            return View(userAddress);
        }

        // POST: UserAddress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAddress = await _context.UserAddress.FindAsync(id);
            if (userAddress != null)
            {
                _context.UserAddress.Remove(userAddress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAddressExists(int id)
        {
            return _context.UserAddress.Any(e => e.UserAdd_Id == id);
        }
    }
}
