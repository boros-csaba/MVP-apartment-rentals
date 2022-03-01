using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IApartmentService
    {
        Task<PaginatedList<Apartment>> GetApartmentsAsync(ApartmentFilter filter);
        Task<Apartment> GetApartmentByIdAsync(string apartmentId);
        Task<Result<Apartment>> CreateApartmentAsync(Apartment apartment);
        Task<Result<Apartment>> UpdateApartmentAsync(Apartment apartment);
        Task<bool> DeleteApartmentAsync(Apartment apartment);
    }
}
