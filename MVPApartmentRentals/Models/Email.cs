using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class Email
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
