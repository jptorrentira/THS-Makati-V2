using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleService.Models
{
    public class ParcelDelivery
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RequestId { get; set; }
        [Required]
        public int ChargingCompanyId { get; set; }
        [Required]
        public int ChargingDepartmentId { get; set; }

        // Parcel Information
        [Required]
        public int ParcelCategoryId { get; set; }
        [Required]
        [Display(Name = "Destination Tag")]
        public int DestinationId { get; set; }
        [Required]
        public string Destination { get; set; }

        [Required]
        public string Recipient { get; set; }
        [Required]
        public string RecipientEmail { get; set; }
        [Required]
        public string RecipientDepartment{ get; set; }

        public DateTime DateTimeToBeReceived { get; set; }
        [NotMapped]
        public string DateToBeReceived { get; set; }
        [NotMapped]
        public string TimeToBeReceived { get; set; }

        [Required]
        public string ParcelDescription { get; set; }
        [Required]
        public string Instruction { get; set; }

        // Tracking Information
        public DateTime ActualDispatchDate { get; set; }
        public DateTime ActualReceivedDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string RejectedBy { get; set; }
        public DateTime RejectedDate { get; set; }
        public string RejectionReason { get; set; }

        public string FiledBy { get; set; }
        public DateTime FiledDate { get; set; }

        [Required]
        public string Status { get; set; }

        // Delivery Imformation
        public int? VehicleListId { get; set; }
        public int? DriverId { get; set; }
        public string DeliveryRemarks { get; set; }


        public ParcelDelivery()
        {
            FiledDate = DateTime.Now;
        }
    }
}
