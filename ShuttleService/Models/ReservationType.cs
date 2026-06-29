using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class ReservationType
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }


    }
}
