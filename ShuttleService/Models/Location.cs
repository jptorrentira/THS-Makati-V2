using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string InsertBy { get; set; }
        public DateTime InsertDate { get; set; }
        
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }

        public ICollection<EmployeeLocation> EmployeeLocations { get; set; }
        public ICollection<DriverLocation> DriverLocations { get; set; }
        public ICollection<VehicleLocation> VehicleLocations { get; set; }
        public Location()
        {
            Status = true;
            InsertDate = DateTime.Now;
        }
    }
}
