using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class Shuttle
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        public long ServiceDateTimeStamp { get; set; }

        [Display(Name = "Service Date")]
        public DateTime ServiceDate { get; set; }

        [Display(Name = "Vehicle")]
        public int VehicleListId { get; set; }
        public virtual VehicleList VehicleList { get; set; } // 1 is to 1

        [Display(Name = "Driver")]
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; } // 1 is to 1

        [Display(Name = "Assembly Area")]
        public string AssemblyArea { get; set; }


        //public List<ShuttlePassenger>   ShuttlePassengers { get; set; } // 1 is to many

        



    }
}
