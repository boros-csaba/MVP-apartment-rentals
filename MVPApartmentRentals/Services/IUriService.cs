using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IUriService
    {
        Uri GetApartmentUri(string apartmentId);
        Uri GetApartmentsUri(ApartmentFilter filter);
        Uri GetUserUri(string userId);
        Uri GetUsersUri(PaginationFilter filter);
        Uri GetUserProfileImageUri(string userId);
        Uri GetEmailConfirmationUri(string userId, string token);
        Uri GetPasswordResetUri(string userId, string token);
        Uri GetInvitationEmailUri();
    }
}
