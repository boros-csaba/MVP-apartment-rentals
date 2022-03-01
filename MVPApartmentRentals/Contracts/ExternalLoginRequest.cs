using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class ExternalLoginRequest
    {
        public string Provider { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
    }
}
