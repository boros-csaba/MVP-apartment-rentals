using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class Result
    {
        public bool Success { get { return !Errors.Any(); } }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }

    public class Result<T>: Result
    {
        public T Data { get; set; }
    }

}
