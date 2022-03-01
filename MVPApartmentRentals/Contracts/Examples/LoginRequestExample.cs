using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts.Examples
{
    public class LoginRequestExample : IExamplesProvider<LoginRequest>
    {
        public LoginRequest GetExamples()
        {
            return new LoginRequest
            {
                Email = "user1@test.com",
                Password = "password1A!"
            };
        }
    }
}
