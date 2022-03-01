using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts.Examples
{
    public class CreateApartmentRequestExample : IExamplesProvider<CreateApartmentRequest>
    {
        public CreateApartmentRequest GetExamples()
        {
            return new CreateApartmentRequest
            {
                Name = "Simple apartment",
                Description = "Small flat on the 9th floot close to the metro station",
                FloorAreaSize = 59,
                PricePerMonth = 210,
                NumberOfRooms = 3,
                Latitude = 47.4555652,
                Longitude = 19.1510055,
                IsAvailable = true,
                RealtorUserId = "e90c0633-91c2-4a5f-8ee0-28d835ed3655"
            };
        }
    }
}
