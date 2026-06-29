using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation

namespace ShuttleService.Models
{
    public class Parameter
    {
        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Value { get; set; } = 1;

        [Required]
        public int Year { get; set; } = 0;

        [Required]
        public string Code { get; set; }
    }
}
