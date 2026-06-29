using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class Driver
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [Display(Name = "License Restriction")]
        public string License_Restriction { get; set; }

        [Required]
        [Display(Name = "License Expiration")]
        public DateTime License_Expiry { get; set; }

        [Required]
        [Display(Name = "Civil Status")]
        public string CivilStatus { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }


        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime EncodeDate { get; set; }


        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }

        [Required]
        public int Status { get; set; } = 1;

        [Required]
        [ForeignKey("CompanyGroupId")]
        [Display(Name = "Company Group")]
        [DefaultValue(1)]
        public int CompanyGroupId { get; set; }
        public virtual CompanyGroup CompanyGroup { get; set; } // 1 is to 1

        public ICollection<DriverLocation> DriverLocations { get; set; }
        [NotMapped]
        public List<int> LocationIds { get; set; } = new List<int>();

    }
}
