using System;
using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class VehicleMonitor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Trip Date & Time")]
        public DateTime TripDateTime { get; set; } 
        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int DriverId { get; set; }
        [Required]
        public string Status { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Remarks { get; set; }
        public string EncodedBy { get; set; }
        public DateTime EncodedDate { get; set; }

        public VehicleMonitor()
        {
            EncodedDate = DateTime.Now;
        }
    }
}
