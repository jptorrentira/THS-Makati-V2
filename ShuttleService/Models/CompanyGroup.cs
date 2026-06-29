using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class CompanyGroup
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Company Group")]
        public string CompanyGroupName { get; set; }
        [DisplayName("Insert Date")]
        public DateTime InsertDate { get; set; }

        public CompanyGroup()
        {
            InsertDate = DateTime.Now;
        }
    }
}
