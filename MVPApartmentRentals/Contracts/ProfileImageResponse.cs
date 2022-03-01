using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class ProfileImageResponse
    {
        public string UserId { get; set; }
        public string Base64Image { get; set; }
    }
}
