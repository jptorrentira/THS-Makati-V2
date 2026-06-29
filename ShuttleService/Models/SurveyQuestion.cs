using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleService.Models
{
    public class SurveyQuestion
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
        public int Order { get; set; }
        public int Status { get; set; } = 1;
    }
}
