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


        // GET: /Desk/Create
        public async Task<IActionResult> Create()
        {          
            return View();
        }


        //POST /Desk/Create/deskName  -- Add a new desk to db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string deskName)
        {
            if (string.IsNullOrWhiteSpace(deskName))
            {
                ModelState.AddModelError("", "Desk name is required.");
                return View();
            }

            var desk = new Desk { Name = deskName };
            _context.Desks.Add(desk);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }     
        //POST /Desk/Delete/id  -- Delete desk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var desk = await _context.Desks.FindAsync(id);
            if (desk == null)
            {
                return NotFound();
            }
            _context.Desks.Remove(desk);
            await _context.SaveChangesAsync();
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
            Debug.WriteLine(desk.Name);
            return View(desk);
        }


        //POST /Desk/Create/deskName  -- Add a new desk to db
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



    }
}
