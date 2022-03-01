using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Services
{
    public interface IEncodingService
    {
        string Encode(string value);
        string Decode(string value);
    }
}
