using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class ErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
