using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema; // for data annotation
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShuttleService.Models
{
    public class TemporaryUser
    {


        [Required]
        public string UserName { get; set; }


        [Required]
        public string Domain { get; set; }


        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} chatacters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Alternate Email")]
        public string AlternateEmail { get; set; }


        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Local Number")]
        public string LocalNumber { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }



    }


    public class LoginViewModel
    {
        [Required]
        public string Domain { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class LocalLoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}

