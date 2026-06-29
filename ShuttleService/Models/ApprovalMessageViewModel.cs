using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class ApprovalMessageViewModel
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferenceNo { get; set; }
        public string Response { get; set; }
        public string SystemName { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
