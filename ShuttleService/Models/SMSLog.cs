using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class SMSLog
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        public long ServiceDateTimeStamp { get; set; }

        [Display(Name = "Version")]
        public int Version { get; set; } = 1;

        public string Process { get; set; }


        [Display(Name = "Status")]
        public int Status { get; set; } = 0;

        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime? ProcessedDate { get; set; }




    }
}
