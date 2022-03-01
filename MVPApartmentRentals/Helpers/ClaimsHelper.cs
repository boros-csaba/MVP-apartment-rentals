using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Helpers
{
    public static class ClaimsHelper
    {
        public static string GetUserId(ClaimsPrincipal user)
        {
            if (user == null) return null;
            var claim = user.Claims.FirstOrDefault(o => o.Type == "userId");
            if (claim == null) return null;
            return claim.Value;
        }
    }
}
