using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class Log
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        public string Process { get; set; }

        [Display(Name = "Processed Date")]
        public DateTime ProcessedDate { get; set; }

        internal static void Information(string v, int length, string fileName, string dbName)
        {
            throw new NotImplementedException();
        }
    }
}
