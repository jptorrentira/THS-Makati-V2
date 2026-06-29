using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class ShuttlePassengerStatus
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        [Display(Name = "Status Description")]
        public string StatusDescription { get; set; }
    }
}
