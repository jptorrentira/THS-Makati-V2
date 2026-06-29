namespace ShuttleService.Models
{
    public class DriverLocation
    {
        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
