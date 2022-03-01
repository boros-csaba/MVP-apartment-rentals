using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Security
{
    public interface IKeyProvider
    {
        SecurityKey GetKey();
        string GetAlgorith();
    }
}
