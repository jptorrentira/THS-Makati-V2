using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation

namespace ShuttleService.Models
{
    public class Trip
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }


        [Display(Name = "Trip Control No")]
        public string TripControlNo { get; set; }


        public long ServiceDateTimeStamp { get; set; }

        [Display(Name = "Service Start Date/Time")]
        public DateTime ServiceStartDate { get; set; }


        [Display(Name = "Service End Date/Time")]
        public DateTime ServiceEndDate { get; set; }

        [Display(Name = "Vehicle Id")]
        public int VehicleListId { get; set; }
        public virtual VehicleList VehicleList { get; set; } // 1 is to 1

        [Required]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Remaining Capacity (AM)")]
        public int RemainingCapacity { get; set; }

        [Required]
        [Display(Name = "Remaining Capacity (PM)")]
        public int RemainingCapacityPM { get; set; }


        [Display(Name = "Driver Id")]
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; } // 1 is to 1

        [Display(Name = "Status")]
        public int Status { get; set; }


        [Required]
        [Display(Name = "Reservation Type Id")]
        public int ReservationTypeId { get; set; }
        public virtual ReservationType ReservationType { get; set; } // 1 is to 1


        [Column(TypeName = "VARCHAR(250)")]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Encoded By")]
        public string EncodedBy { get; set; }


        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }


        [Display(Name = "Encode Date")]
        public DateTime? EncodeDate { get; set; }


        [Display(Name = "Modify Date")]

        public DateTime? ModifyDate { get; set; }



        [Display(Name = "Cancelled By")]
        public string CancelledBy { get; set; }

        [Display(Name = "Cancelled Datetime")]
        public DateTime? CancelledDatetime { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancelReason { get; set; }


        [Display(Name = "Email Status")]
        public int EmailStatus { get; set; } = 0;

        [Display(Name = "Email By")]
        public string EmailBy { get; set; }

        [Display(Name = "Email Datetime")]
        public DateTime? EmailDatetime { get; set; }

        [Display(Name = "Email Version")]
        public int EmailVersion { get; set; } = 1;


        [Display(Name = "SMS Status")]
        public int SMSStatus { get; set; } = 0;

        [Display(Name = "SMS By")]
        public string SMSBy { get; set; }

        [Display(Name = "SMS Datetime")]
        public DateTime? SMSDatetime { get; set; }

        [Display(Name = "SMS Version")]
        public int SMSVersion { get; set; } = 1;

    }
}
