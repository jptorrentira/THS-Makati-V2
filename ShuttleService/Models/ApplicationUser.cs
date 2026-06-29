using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace ShuttleService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "VARCHAR(80)")]
        public string DisplayName { get; set; }

        //[ForeignKey("Department2")]// model key when model has different name

        [ForeignKey("DepartmentListId")]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        public virtual DepartmentList DepartmentList { get; set; } // 1 is to 1

        [Column(TypeName = "VARCHAR(20)")]
        public string Domain { get; set; }


        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } // 1 is to 1

        //public List<UserTask> UserTasks { get; set; } // 1 is to many

        public int Status { get; set; } = 1;
    }
}
    