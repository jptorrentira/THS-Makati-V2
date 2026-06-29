using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleService.Models
{
    public class OriginDestination
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OriginDestinationName { get; set; }

        [Required]
        [ForeignKey("CompanyGroupId")]
        [Display(Name = "Company Group")]
        [DefaultValue(1)]
        public int CompanyGroupId { get; set; }
        public virtual CompanyGroup CompanyGroup { get; set; }

        public string InsertBy { get; set; }
        public DateTime InsertDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }

        public OriginDestination()
        {
            InsertDate = DateTime.Now;
        }
    }
}
