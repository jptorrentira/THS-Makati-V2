using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class SmsViewModel
    {
        public string MobileNo { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public DateTime ReplyDateTime { get; set; }
        public string Remarks { get; set; }
    }
}
