using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IPaginationService
    {
        Task<PaginatedList<T>> GetPaginatedListAsync<T>(IQueryable<T> query, PaginationFilter filter);
    }
}
