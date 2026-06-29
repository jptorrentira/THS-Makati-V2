namespace ShuttleService.Models
{
    public class ParcelReport
    {
        public int No { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string RequestId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Driver { get; set; }
        public string PlateNumber { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceivedDate { get; set; }
    }
}
