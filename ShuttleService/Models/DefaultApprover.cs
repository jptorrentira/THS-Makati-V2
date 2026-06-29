using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation

namespace ShuttleService.Models
{
    public class DefaultApprover
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeNo")]
        [Display(Name = "Approver Employee No")]
        public string ApproverEmployeeNo { get; set; }
        public virtual Employee ApproverEmployee { get; set; } // 1 is to 1


        [Required]
        [ForeignKey("EmployeeNo")]
        [Display(Name = "Employee No")]
        public string EmployeeNo { get; set; }
        public virtual Employee Employee { get; set; } // 1 is to 1


        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime? EncodeDate { get; set; }


        [Display(Name = "Modify Date")]

        public DateTime? ModifyDate { get; set; }



    }
}
