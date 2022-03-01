using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class PaginationFilter
    {
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
