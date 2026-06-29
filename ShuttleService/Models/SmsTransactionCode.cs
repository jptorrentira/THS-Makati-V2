using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class SmsTransactionCode
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string ReferenceNo { get; set; }
        public string ApproverEmployeeNo { get; set; }
        public string OriginalApproverEmployeeNo { get; set; }
        public int Status { get; set; } = 1;
        public int ReservationTypeId { get; set; }

        public string Remarks { get; set; }
    }
}
