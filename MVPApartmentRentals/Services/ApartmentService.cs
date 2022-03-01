using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;

        public ApartmentService(DataContext context, IPaginationService paginationService)
        {
            this.context = context ?? throw new ArgumentNullException("context");
            this.paginationService = paginationService ?? throw new ArgumentNullException("paginationService");
        }

        public async Task<PaginatedList<Apartment>> GetApartmentsAsync(ApartmentFilter filter)
        {
            var query = context.Apartments.Include(a => a.Realtor).OrderByDescending(a => a.AddedDate).AsQueryable();
            if (filter.UserId != null)
            {
                var userExists = await context.Users.AnyAsync();
                if (!userExists)
                {
                    return new PaginatedList<Apartment> { Errors = new string[] { "Invaid user Id!" } };
                }
                query = query.Where(a => a.Realtor.Id == filter.UserId);
            }
            if (filter.OnlyAvailable) query = query.Where(a => a.Status == ApartmentStatusEnum.Available);
            if (filter.MinArea.HasValue) query = query.Where(a => a.FloorAreaSize >= filter.MinArea.Value);
            if (filter.MaxArea.HasValue) query = query.Where(a => a.FloorAreaSize <= filter.MaxArea.Value);
            if (filter.MinRooms.HasValue) query = query.Where(a => a.NumberOfRooms >= filter.MinRooms.Value);
            if (filter.MaxRooms.HasValue) query = query.Where(a => a.NumberOfRooms <= filter.MaxRooms.Value);
            if (filter.MinPrice.HasValue) query = query.Where(a => a.PricePerMonth >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) query = query.Where(a => a.PricePerMonth <= filter.MaxPrice.Value);

            return await paginationService.GetPaginatedListAsync(query, filter);
        }

        public async Task<Apartment> GetApartmentByIdAsync(string apartmentId)
        {
            return await context.Apartments.Include(a => a.Realtor).SingleOrDefaultAsync(a => a.Id == apartmentId);
        }

        public async Task<Result<Apartment>> CreateApartmentAsync(Apartment apartment)
        {
            await context.AddAsync(apartment);
            var created = await context.SaveChangesAsync();
            if (created > 0)
            {
                return new Result<Apartment> { Data = apartment };
            }
            return new Result<Apartment> { Errors = new string[] { "Apartment creation failed!" } };
        }

        public async Task<Result<Apartment>> UpdateApartmentAsync(Apartment apartment)
        {
            context.Apartments.Update(apartment);
            var updated = await context.SaveChangesAsync();
            if (updated > 0)
            {
                return new Result<Apartment> { Data = apartment };
            }
            return new Result<Apartment> { Errors = new string[] { "Apartment creation failed!" } };
        }

        public async Task<bool> DeleteApartmentAsync(Apartment apartment)
        {
            context.Apartments.Remove(apartment);
            var deleted = await context.SaveChangesAsync();
            return deleted > 0;
        }
        
    }
}
