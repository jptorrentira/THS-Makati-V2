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
    public class DriverPassenger
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }


        public long ServiceDateTimeStamp { get; set; }

        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }


        [Display(Name = "Service Date")]
        public DateTime? ServiceDate { get; set; }
        

        public int ShuttleId { get; set; }




        [Display(Name = "Passenger Type")]
        public int PassengerTypeId { get; set; }
        public virtual PassengerType PassengerType { get; set; } // 1 is to 1

       
        public string EmployeeNo { get; set; }


        [Display(Name = "Created By")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } // 1 is to 1


        [Display(Name = "Company")]
        public int CompanyListId { get; set; }
        public virtual CompanyList CompanyList { get; set; } // 1 is to 1

        [Display(Name = "Other Company")]
        public string CompanyOther { get; set; }


        [Display(Name = "Charging Company")]
        public int ChargingCompanyId { get; set; }
        public virtual ChargingCompany ChargingCompany { get; set; } // 1 is to 1
        
        [Display(Name = "Charging Department")]
        public int ChargingDepartmentId { get; set; }
        public virtual ChargingDepartment ChargingDepartment { get; set; } // 1 is to 1

    
        public string Name { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
      
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Position { get; set; }


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
        [Display(Name = "Origin Tag")]
        public int OriginId { get; set; }


        [Required]
        [Display(Name = "Destination Tag")]
        public int DestinationId { get; set; }


        [Required]
        [Display(Name = "Origin")]
        public string Origin { get; set; }


        [Required]
        [Display(Name = "Destination")]
        public string Destination { get; set; }


        //[Display(Name = "Origin")]
        //public int ChargingDepartmentId { get; set; }
        //public virtual ChargingDepartment ChargingDepartment { get; set; } // 1 is to 1



        //public Boolean Breakfast { get; set; }

        //[Display(Name = "AM Snack")]
        //public Boolean AmSnack { get; set; }

        //[Display(Name = "PM Snack")]
        //public Boolean PmSnack { get; set; }
        //public Boolean Lunch { get; set; }
        //public Boolean Dinner { get; set; }



        //[Display(Name = "Lodging From")]
        //public DateTime? LodgingFrom { get; set; }

        //[Display(Name = "Lodging To")]
        //public DateTime? LodgingTo { get; set; }

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


        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

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


        public string DriverPassengerHeaderId { get; set; }

        public virtual DriverPassengerHeader DriverPassengerHeader { get; set; }



        [Display(Name = "Cancelled By")]
        public string CancelledBy { get; set; }

        [Display(Name = "Cancelled Datetime")]
        public DateTime? CancelledDatetime { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancelReason { get; set; }



        [Display(Name = "Declined By")]
        public string DeclinedBy { get; set; }

        [Display(Name = "Declined Datetime")]
        public DateTime? DeclinedDatetime { get; set; }

        [Display(Name = "Declined Reason")]
        public string DeclinedReason { get; set; }




        [Display(Name = "Reserved By")]
        public string ReservedBy { get; set; }

        [Display(Name = "Reserved Datetime")]
        public DateTime? ReservedDatetime { get; set; }


        [Display(Name = "Removed By")]
        public string RemovedBy { get; set; }

        [Display(Name = "Removed Datetime")]
        public DateTime? RemovedDatetime { get; set; }


        [Display(Name = "Removed Remarks")]
        public string RemovedRemarks { get; set; }



        [Required]
        public string Nationality { get; set; }


        [Required]
        public string Gender { get; set; }


        [Display(Name = "SMS ID")]
        public string SmsId { get; set; }


        [Display(Name = "Original Approver")]
        public string OriginalApproverEmployeeNo { get; set; }

        public int SMSRevision { get; set; } = 0;

        [Display(Name = "Unplanned Trip")]
        public int UnplannedTrip { get; set; } = 0; // 0 if no // 1 if yes
    }
}
