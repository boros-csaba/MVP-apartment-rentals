using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public int TotalCount { get; set; }
        public string Prev { get; set; }
        public string Next { get; set; }
    }
}
