using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class ParcelRequestIdParameter
    {
        [Key]
        public int id { get; set; }
        public int Year { get; set; }
        public int LastNumber { get; set; }
    }
}
