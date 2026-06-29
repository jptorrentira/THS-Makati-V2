using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class DepartmentList
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Required]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Company")]

        public int CompanyListId { get; set; }

        [Display(Name = "Company Group")]
        public int CompanyGroupId { get; set; }


        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime EncodeDate { get; set; }


        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }
    }
}
