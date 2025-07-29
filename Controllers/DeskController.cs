using DeskBookingApplication.Data;
using DeskBookingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DeskBookingApplication.Controllers
{
    [Authorize(Roles = "Manager")]
    public class DeskController : Controller
    {
        private readonly DeskBookingAuthDbContext _context;

        public DeskController(DeskBookingAuthDbContext context)
        {
            _context = context;
        }

        // GET: /Desk/Manage - Displays all desks
        public async Task<IActionResult> Manage()
        {
            var desks = await _context.Desks.ToListAsync();
            return View(desks);
        }

        //POST /Desk/Create  -- Adds a new desk to db - automatically gives name "Desk (next number in list)" - eg Desk 1 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            //Gets all the desk names - All have to begin with "Desk" "
            var deskNames = await _context.Desks
                .Where(d => d.Name.StartsWith("Desk "))
                .Select(d => d.Name)
                .ToListAsync();

            //Extract the numbers that come after "Desk" eg Desk 1
            var deskNumbers = deskNames
                .Select(name =>
                {
                    var parts = name.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                    {
                        return num;
                    }
                    return 0;
                })
                .ToList();

            var nextNumber = deskNumbers.Any() ? deskNumbers.Max() + 1 : 1;
            var deskName = $"Desk {nextNumber}";

            //Creates the new desk in desk database, adds and saves it
            var desk = new Desk
            {
                Name = deskName,
                IsActive = true
            };

            _context.Desks.Add(desk);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{deskName} has been created!";
            return RedirectToAction(nameof(Manage));
        }

        // GET: /Desk/Edit/ID  -- Prefills selected desk name
        public async Task<IActionResult> Edit(int id)
        {
            var desk = await _context.Desks.FindAsync(id);
            if (desk == null)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = $"Desk description has been updated!";
            return View(desk);
        }


        //POST /Desk/Create/deskName  -- Edit desk 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Desk desk)
        {
            if (!ModelState.IsValid)
            {
                return View(desk);
            }

            _context.Update(desk);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

        //POST /Desk/activate/id  -- Activate desk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var desk = await _context.Desks.FindAsync(id);

            if (desk == null)
            {
                return NotFound();
            }
            desk.IsActive = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Desk has been activated!";
            return RedirectToAction(nameof(Manage));
        }


        //POST /Desk/Deactivate/id  -- Deactivate desk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var desk = await _context.Desks.FindAsync(id);

            if (desk == null)
            {
                return NotFound();
            }

            //Check if selected desk has upcoming bookings - error message displays if true
            bool hasUpcomingBookings = await _context.DeskBookings
                .AnyAsync(b => b.DeskId == id && b.BookingDate >= DateTime.Today);

            if (hasUpcomingBookings)
            {
                TempData["ErrorMessage"] = "Desk cannot be deactivated as it has upcoming bookings!";
                return RedirectToAction(nameof(Manage));
            }

            desk.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Desk has been deactivated!";
            return RedirectToAction(nameof(Manage));
        }
    }
}
