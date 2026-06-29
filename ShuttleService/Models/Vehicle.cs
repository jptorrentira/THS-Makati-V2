using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class Vehicle
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        //[Display(Name = "Description Name")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [Display(Name = "Coding Day")]
        public DateTime CodingDay { get; set; }

        [Required]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

    }
}
