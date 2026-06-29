using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Models
{
    public class ApplicationRole : IdentityRole
    {
        public bool Status { get; set; }

        public static implicit operator ApplicationRole(TemporaryUser v)
        {
            throw new NotImplementedException();
        }
    }
}
