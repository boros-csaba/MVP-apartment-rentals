using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Contracts
{
    public class UpdateApartmentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double FloorAreaSize { get; set; }
        public decimal PricePerMonth { get; set; }
        public int NumberOfRooms { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsAvailable { get; set; }
        public string RealtorUserId { get; set; }
    }
}
