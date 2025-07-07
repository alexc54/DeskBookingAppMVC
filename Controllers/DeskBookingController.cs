using DeskBookingApplication.Areas.Identity.Data;
using DeskBookingApplication.Data;
using DeskBookingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DeskBookingApplication.Controllers
{
    [Authorize]
    public class DeskBookingController : Controller
    {
        private readonly DeskBookingAuthDbContext _context;
        private readonly UserManager<DeskBookingApplicationUser> _userManager;

        public DeskBookingController(
            DeskBookingAuthDbContext context,
            UserManager<DeskBookingApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET /DeskBooking/Book - Displays avaliable desks
        public async Task<IActionResult> Book(DateTime? date)
        {
            if (date == null)
                date = DateTime.Today;

            //Get desks already booked for inputted date
            var bookedDeskIds = await _context.DeskBookings
                .Where(b => b.BookingDate.Date == date.Value.Date)
                .Select(b => b.DeskId)
                .ToListAsync();

            //Query that gets desks that are not booked
            var availableDesks = await _context.Desks
                .Where(d => !bookedDeskIds.Contains(d.Id))
                .ToListAsync();

            ViewBag.Date = date.Value.ToString("yyyy-MM-dd");
            return View(availableDesks);
        }

        //POST /DeskBooking/Book  -- Making a booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int deskId, DateTime date)
        {
            var user = await _userManager.GetUserAsync(User);

            //Check if booking date input is in the past
            if (date.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "You cannot book a desk for past date!");
            }

            //Check if user has already booked a desk on inputted date
            bool alreadyBooked = await _context.DeskBookings
                .AnyAsync(b => b.UserId == user.Id && b.BookingDate.Date == date.Date);

            if (alreadyBooked)
            {
                //Error will display on the screen telling the user they have already booked desk on this date
                ModelState.AddModelError("", "You have already booked a desk on this date!");

                //Display available desks again
                var bookedDeskIds = await _context.DeskBookings
                    .Where(b => b.BookingDate.Date == date.Date)
                    .Select(b => b.DeskId)
                    .ToListAsync();

                var availableDesks = await _context.Desks
                    .Where(d => !bookedDeskIds.Contains(d.Id))
                    .ToListAsync();

                ViewBag.Date = date.ToString("yyyy-MM-dd");
                
                return View(availableDesks);
            }

            //Creates and saves new booking if no errors
            var booking = new DeskBooking
            {
                DeskId = deskId,
                BookingDate = date.Date,
                UserId = user.Id
            };

            _context.DeskBookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }


        //GET /DeskBooking/MyBookings  - Displays users bookings
        public async Task<IActionResult> MyBookings()
        {
            //Gets current user
            var user = await _userManager.GetUserAsync(User);

            //Finds upcoming bookings done by this user (today included)
            var myBookings = await _context.DeskBookings
                .Include(b => b.Desk)
                .Where(b => b.UserId == user.Id && b.BookingDate.Date>= DateTime.Today)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            return View(myBookings);
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
            return RedirectToAction(nameof(MyBookings));
        }

        //GET /DeskBooking/MyBookingHistory  - Displays users previous bookings
        public async Task<IActionResult> MyBookingHistory()
        {
            //Gets current user
            var user = await _userManager.GetUserAsync(User);

            //Finds all bookings done by this user previous to todays date
            var myBookings = await _context.DeskBookings
            .Include(b => b.Desk)
                .Where(b => b.UserId == user.Id && b.BookingDate.Date < DateTime.Today)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            return View(myBookings);
        }

    }

    




}
