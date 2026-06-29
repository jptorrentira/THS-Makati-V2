using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleService.Models
{
    public class SurveyAnswer
    {
        [Key]
        public int Id { get; set; }

        public int SurveyTransactionId { get; set; }
        public virtual SurveyTransaction SurveyTransaction { get; set; }


        public int SurveyQuestionId { get; set; }
        public virtual SurveyQuestion SurveyQuestion { get; set; }

        public int AnswerScore { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; } = 1;
    }
}
