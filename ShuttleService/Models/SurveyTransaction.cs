using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleService.Models
{
    public class SurveyTransaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionId { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee{ get; set; } // 1 is to 1

        public string SurveyHash { get; set; }

        public int Status { get; set; } = 1;

        public int IsAnswered { get; set; } = 0;

        public DateTime GeneratedDateTime { get; set; } 

        public DateTime? AnsweredDateTime { get; set; } 


    }
}

