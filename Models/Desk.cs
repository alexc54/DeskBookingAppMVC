namespace DeskBookingApplication.Models
{
    public class Desk
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
