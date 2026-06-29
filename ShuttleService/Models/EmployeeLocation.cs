namespace ShuttleService.Models
{
    public class EmployeeLocation
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
