using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class MaintenanceLog
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [Display(Name = "Vehicle Id")]
        public int VehicleListId { get; set; }
        public virtual VehicleList VehicleList { get; set; } // 1 is to 1

        public string Process { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime ProcessedDate { get; set; }


        [Display(Name = "Maintenance From")]
        public DateTime? MaintenanceFrom { get; set; }

        [Display(Name = "Maintenance To")]
        public DateTime? MaintenanceTo { get; set; }

        [Display(Name = "Maintenance Remarks")]
        public string MaintenanceRemarks { get; set; }

        [Display(Name = "Maintenance ControlNo")]
        public string MaintenanceControlNo { get; set; }

    }
}
