using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts.Examples
{
    public class LoginErrorResponseExample : IExamplesProvider<LoginErrorResponse>
    {
        public LoginErrorResponse GetExamples()
        {
            return new LoginErrorResponse
            {
                Errors = new string[] { "Email or password is incorrect!" }
            };
        }
    }
}
