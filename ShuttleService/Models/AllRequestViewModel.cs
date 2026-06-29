using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class AllRequestViewModel
    {

        public string TransactionId { get; set; }
        public string SmsId { get; set; }
        public int Status { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDatetime { get; set; }

        public string RequestType { get; set; } //driver or shuttle

    }
}
