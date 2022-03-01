using System;
using System.Collections.Generic;
using MVPApartmentRentals.Contracts;
using MVPApartmentRentals.Services;

namespace MVPApartmentRentals.Helpers
{
    public class PaginationHelper
    {

        public static PaginatedResponse<T> CreatePaginatedResponse<T, Y>(IEnumerable<T> data, int totalCount, Y filter, Func<Y, string> uriGetter) where Y: Models.PaginationFilter
        {
            var result = new PaginatedResponse<T>
            {
                Data = data,
                TotalCount = totalCount,
                Limit = filter.Limit,
                Offset = filter.Offset
            };

            if (filter.Limit.HasValue)
            {
                var limit = filter.Limit.Value;
                var offset = filter.Offset ?? 0;
                if (offset > 0)
                {
                    filter.Offset = Math.Max(offset - limit, 0);
                    result.Prev = uriGetter(filter);
                }
                if (totalCount > offset + limit)
                {
                    filter.Offset = offset + limit;
                    result.Next = uriGetter(filter);
                }
            }

            return result;
        }
    }
}
