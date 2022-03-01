using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class PaginatedList<T>: Result<List<T>>
    {
        public int TotalCount { get; set; }
    }
}
