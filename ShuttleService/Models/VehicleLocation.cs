namespace ShuttleService.Models
{
    public class VehicleLocation
    {
        public int VehicleId { get; set; }
        public VehicleList Vehicle { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
