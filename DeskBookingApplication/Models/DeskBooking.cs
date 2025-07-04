using DeskBookingApplication.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace DeskBookingApplication.Models
{
    public class DeskBooking
    {        public int Id { get; set; }

        public string UserId { get; set; }
        public DeskBookingApplicationUser User { get; set; }

        public int DeskId { get; set; }
        public Desk Desk { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
    }
}
