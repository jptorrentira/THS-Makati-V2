using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation

//model singular
//dbset plural
//controller plural

namespace ShuttleService.Models
{
    public class DriverPassengerHeader
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }


        public long ServiceDateTimeStamp { get; set; }

        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }


        [Display(Name = "Temporary Transaction ID")]
        public string TempTransactionId { get; set; }


        [Display(Name = "Service Date")]
        public DateTime? ServiceDate { get; set; }
        

        public int ShuttleId { get; set; }


        [Display(Name = "Created By")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } // 1 is to 1

    

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Purpose { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(250)")]
        public string Remarks { get; set; }

        //[Display(Name = "Trip Type")]
        //public int TripTypeId { get; set; }
        //public virtual TripType TripType { get; set; } // 1 is to 1

        [Required]
        [Display(Name = "Trip Time From")]
        public string TripTimeFrom { get; set; }


        [Required]
        [Display(Name = "Trip Time To")]
        public string TripTimeTo { get; set; }


        [Required]
        [Display(Name = "Origin")]
        public string Origin { get; set; }


        [Required]
        [Display(Name = "Destination")]
        public string Destination { get; set; }



        [Required]
        [Display(Name = "Origin Tag")]
        public int OriginId { get; set; }


        [Required]
        [Display(Name = "Destination Tag")]
        public int DestinationId { get; set; }


        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime? EncodeDate { get; set; }


        [Display(Name = "Modify Date")]

        public DateTime? ModifyDate { get; set; }


        [Display(Name = "Submit Date")]

        public DateTime? SubmitDate { get; set; }


        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Initial Approver")]
        public string InitialApproverEmployeeNo { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Datetime")]
        public DateTime? ApprovedDatetime { get; set; }



        [Display(Name = "Service Type")]
        public int ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; } // 1 is to 1



        [Required]
        public string Instructions { get; set; }





        public List<DriverPassenger> DriverPassengers{ get; set; } // 1 is to many


        [Display(Name = "Unplanned Trip")]
        public int UnplannedTrip { get; set; } = 0; // 0 if no // 1 if yes

        public string DestinationTag { get; set; }
        public string OriginTag { get; set; }


    }
}
