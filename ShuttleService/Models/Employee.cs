    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // for data annotation

namespace ShuttleService.Models
{
    public class Employee
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

 
        [Required]
        [Display(Name = "Employee No")]
        public string EmployeeNo { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

 
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        public string Position { get; set; }

        [Required]
        [ForeignKey("CompanyListId")]
        [Display(Name = "Company")]
        public int CompanyListId { get; set; } 
        public virtual CompanyList CompanyList { get; set; } // 1 is to 1

        [Required]
        [ForeignKey("CompanyGroupId")]
        [Display(Name = "Company Group")]
        [DefaultValue(1)]
        public int CompanyGroupId { get; set; }
        public virtual CompanyGroup CompanyGroup { get; set; } // 1 is to 1

        public ICollection<EmployeeLocation> EmployeeLocations { get; set; }
        [NotMapped]
        public List<int> LocationIds { get; set; } = new List<int>();

        [Required]
        [Display(Name = "Department")]
        public int DepartmentListId { get; set; }

        [Required]
        [Display(Name = "Charging Company")]
        public int ChargingCompanyId { get; set; }

        [Required]
        [Display(Name = "Charging Department")]
        public int ChargingDepartmentId { get; set; }


        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Nationality")]
        public int NationalityId { get; set; }
        public virtual Nationality Nationality { get; set; } // 1 is to 1


        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [Display(Name = "Local Number")]
        public string LocalNumber { get; set; }

        [Required]
        [Display(Name = "Company Email")]
        public string CompanyEmail { get; set; }

        [Required]
        [Display(Name = "Alternative Email")]
        public string AlternativeEmail { get; set; }


        //[Required]
        //[ForeignKey("EmployeeNo")]
        //[Display(Name = "Supervisor Id")]
        //public string SupervisorId { get; set; }
        //public virtual Employee Supervisor { get; set; } // 1 is to 1

        [Required]
        [Display(Name = "Immediate Supervisor")]
        public string SupervisorEmployeeNo { get; set; }

        [Required]
        public int Status { get; set; } = 1;



        [Display(Name = "Is Immediate Head")]
        public Boolean IsImmediateHead { get; set; }


        [Display(Name = "Alternate Immediate Head")]
        public string AlternateImmediateHead { get; set; }


        [Display(Name = "Alternate Immediate Head Valid From")]
        public DateTime? AlternateImmediateHeadValidFrom { get; set; }


        [Display(Name = "Alternate Immediate Head Valid To")]
        public DateTime? AlternateImmediateHeadValidTo { get; set; }


        [Display(Name = "Alternate Immediate Head Validity")]
        public Boolean AlternateImmediateHeadValidity { get; set; } = false;



        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime EncodeDate { get; set; }


        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }


        [Required]
        public int is_recomputed { get; set; } = 1;
    }
}
