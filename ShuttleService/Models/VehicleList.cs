using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class VehicleList
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        //[Display(Name = "Description Name")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }


        [Display(Name = "Coding Day")]
        public int CodingDay { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        [Required]
        [ForeignKey("CompanyGroupId")]
        [Display(Name = "Company Group")]
        [DefaultValue(1)]
        public int CompanyGroupId { get; set; } = 0;
        public virtual CompanyGroup CompanyGroup { get; set; } // 1 is to 1

        public ICollection<VehicleLocation> VehicleLocations { get; set; }
        [NotMapped]
        public List<int> LocationIds { get; set; } = new List<int>();

        [Required]
        public int Status { get; set; } = 1;

        //[Display(Name = "Maintenance From")]
        //public DateTime? MaintenanceFrom { get; set; }

        //[Display(Name = "Maintenance To")]
        //public DateTime? MaintenanceTo { get; set; }

        //[Display(Name = "Maintanance Remarks")]
        //public string MaintananceRemarks { get; set; }

        //[Display(Name = "Under Maintanance")]
        //public Boolean UnderMaintanance { get; set; } = false;

        //[Display(Name = "Maintanance ControlNo")]
        //public string MaintananceControlNo { get; set; }
        


    }
}
