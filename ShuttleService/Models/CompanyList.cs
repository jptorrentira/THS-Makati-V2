using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class CompanyList
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }



        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime EncodeDate { get; set; }


        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }

        [Required]
        [ForeignKey("CompanyGroupId")]
        [Display(Name = "Company Group")]
        [DefaultValue(1)]
        public int CompanyGroupId { get; set; }
        public virtual CompanyGroup CompanyGroup { get; set; } // 1 is to 1
    }
}
