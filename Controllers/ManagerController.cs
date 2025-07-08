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

        //GET /manager/ManageBookings  - Displays bookings for all users
        public async Task<IActionResult> ManageBookings()
        {
            //Gets current user
            var user = await _userManager.GetUserAsync(User);

            //Finds upcoming bookings done by all users (today included)
            var AllBookings = await _context.DeskBookings
                .Include(b => b.Desk)
                .Include(b => b.User)
                .Where(b => b.BookingDate.Date >= DateTime.Today)
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