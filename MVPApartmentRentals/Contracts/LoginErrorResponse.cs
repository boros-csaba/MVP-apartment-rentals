using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class LoginErrorResponse: ErrorResponse
    {
        public bool EmailConfirmed { get; set; }
        public string UserId { get; set; }
    }
}
