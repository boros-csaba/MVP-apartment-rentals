using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts.Examples
{
    public class UserRegistrationRequestExample : IExamplesProvider<UserRegistrationRequest>
    {
        public UserRegistrationRequest GetExamples()
        {
            return new UserRegistrationRequest
            {
                Email = "user1@test.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password1A!",
                ConfirmedPassword = "password1A!"
            };
        }
    }
}
