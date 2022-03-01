using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Roles { get; set; }
        public bool IsBlocked { get; set; }
        public string ProfileImageLink { get; set; }
    }
}
