using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Security
{
    public class CertificateKeyProvider: IKeyProvider
    {

        public SecurityKey GetKey()
        {
            var certificate = new X509Certificate2("Security/api-cert.pfx", "681saa844v");
            return new X509SecurityKey(certificate);
        }

        public string GetAlgorith()
        {
            return SecurityAlgorithms.RsaSha256Signature;
        }
    }
}
