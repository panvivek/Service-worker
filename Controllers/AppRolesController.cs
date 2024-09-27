using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ServiceWorkerWebsite
{
    public class AppRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync(); // Use ToListAsync for better async handling
            return View(roles); // Pass the roles to the view
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (!await _roleManager.RoleExistsAsync(model.Name))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Name));
            }

            return RedirectToAction("Index");
        }
    }
}