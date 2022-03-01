using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfileImage { get; set; }
        [NotMapped]
        public string[] Roles { get; set; }
    }
}
