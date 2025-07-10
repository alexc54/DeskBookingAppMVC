using DeskBookingApplication.Areas.Identity.Data;
using DeskBookingApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingApplication.Controllers
{
    [Authorize(Roles="Manager")]
    public class ManagerController : Controller
    {
        private readonly DeskBookingAuthDbContext _context;
        private readonly UserManager<DeskBookingApplicationUser> _userManager;

        public ManagerController(
            DeskBookingAuthDbContext context,
            UserManager<DeskBookingApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET /manager/ManageBookings  - Displays bookings for all users - unless search bar used 
        public async Task<IActionResult> ManageBookings(string searchString, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _context.DeskBookings
                .Include(b => b.Desk)
                .Include(b => b.User)
                .Where(b => b.BookingDate.Date >= DateTime.Today);

            //If user enters into search bar, filter by first or last name of employee 
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b =>
                    b.User.FirstName.Contains(searchString) ||
                    b.User.LastName.Contains(searchString));
            }


            // Filter by dateFrom if provided
            if (dateFrom.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date >= dateFrom.Value.Date);
            }

            // Filter by dateTo if provided
            if (dateTo.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date <= dateTo.Value.Date);
            }



            //Finds upcoming bookings done by all users (today included)
            var AllBookings = await query                
                .OrderBy(b => b.BookingDate)
                .ToListAsync();            

            return View(AllBookings);
        }

        //POST /DeskBooking/Cancel/id  -- Cancel/Delete booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.DeskBookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            _context.DeskBookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBookings));
        }

    }
}