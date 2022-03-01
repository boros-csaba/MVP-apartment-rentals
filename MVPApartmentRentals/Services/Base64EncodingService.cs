using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Services
{
    public class Base64EncodingService : IEncodingService
    {

        public string Decode(string value)
        {
            string base64String = value.Replace('_', '/').Replace('-', '+');
            switch (value.Length % 4)
            {
                case 2: base64String += "=="; break;
                case 3: base64String += "="; break;
            }
            return base64String;
        }

        public string Encode(string value)
        {
            char[] padding = { '=' };
            return value.TrimEnd(padding).Replace('+', '-').Replace('/', '_');
        }
    }
}
