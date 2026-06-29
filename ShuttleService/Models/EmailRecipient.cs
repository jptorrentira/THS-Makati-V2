using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ShuttleService.Models
{
    public class EmailRecipient
    {

        [Key] // must add System.ComponentModel.DataAnnotations; (TRAINING)
        public int Id { get; set; }
        
        public string Email { get; set; }

        public string Group { get; set; }

        public string Status { get; set; }





    }
}
