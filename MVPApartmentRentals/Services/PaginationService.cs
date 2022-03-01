using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginatedList<T>> GetPaginatedListAsync<T>(IQueryable<T> query, PaginationFilter filter)
        {
            if (filter.Limit.HasValue && filter.Limit.Value <= 0)
            {
                return new PaginatedList<T> { Errors = new string[] { "The limit must be greater than 0!" } };
            }
            if (filter.Offset.HasValue && filter.Offset < 0)
            {
                return new PaginatedList<T> { Errors = new string[] { "The offset must be greater than or equeal to 0!" } };
            }
            var totalCount = await query.CountAsync();
            if (filter.Offset.HasValue) query = query.Skip(filter.Offset.Value);
            if (filter.Limit.HasValue) query = query.Take(filter.Limit.Value);
            var data = await query.ToListAsync();

            return new PaginatedList<T> { Data = data, TotalCount = totalCount };
        }
    }
}
