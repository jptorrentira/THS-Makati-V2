using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class Destination
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        public string DestinationName { get; set; }
    }
}
