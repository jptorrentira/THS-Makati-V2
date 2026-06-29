using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class ParcelCategory
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }

        public string InsertBy { get; set; }
        public DateTime InsertDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }

    }
}
