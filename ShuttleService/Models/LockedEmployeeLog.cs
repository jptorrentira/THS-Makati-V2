using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class LockedEmployeeLog
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } // 1 is to 1

        public string Process { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime ProcessedDate { get; set; }


        [Display(Name = "Locked From")]
        public DateTime? LockedFrom { get; set; }

        [Display(Name = "Locked To")]
        public DateTime? LockedTo { get; set; }

        [Display(Name = "Locked Remarks")]
        public string LockedRemarks { get; set; }

        [Display(Name = "Locked ControlNo")]
        public string LockedControlNo { get; set; }

    }
}
