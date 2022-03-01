using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public class UriService : IUriService
    {
        private readonly string baseUri;
        private readonly string emailConfirmationLinkBase;
        private readonly string passwordResetLinkBase;
        private readonly string invitationLinkBase;

        public UriService(string baseUri, string emailConfirmationLinkBase, string passwordResetLinkBase, string invitationLinkBase)
        {
            this.baseUri = baseUri ?? throw new ArgumentNullException("baseUri");
            this.emailConfirmationLinkBase = emailConfirmationLinkBase ?? throw new ArgumentNullException("emailConfirmationLinkBase");
            this.passwordResetLinkBase = passwordResetLinkBase ?? throw new ArgumentNullException("passwordResetLinkBase");
            this.invitationLinkBase = invitationLinkBase ?? throw new ArgumentNullException("invitationLinkBase");
        }

        public Uri GetApartmentUri(string apartmentId)
        {
            return new Uri(baseUri + ApiRoutes.Apartments.Get.Replace("{apartmentId}", apartmentId));
        }

        public Uri GetApartmentsUri(ApartmentFilter filter)
        {
            var urlParts = new List<string>();
            if (filter.Limit.HasValue) urlParts.Add($"limit={filter.Limit.Value}");
            if (filter.Offset.HasValue) urlParts.Add($"offset={filter.Offset.Value}");
            if (filter.MinArea.HasValue) urlParts.Add($"minArea={filter.MinArea.Value}");
            if (filter.MaxArea.HasValue) urlParts.Add($"maxArea={filter.MaxArea.Value}");
            if (filter.MinRooms.HasValue) urlParts.Add($"minRooms={filter.MinRooms.Value}");
            if (filter.MaxRooms.HasValue) urlParts.Add($"maxRooms={filter.MaxRooms.Value}");
            if (filter.MinPrice.HasValue) urlParts.Add($"minPrice={filter.MinPrice.Value}");
            if (filter.MaxPrice.HasValue) urlParts.Add($"maxPrice={filter.MaxPrice.Value}");

            var url = baseUri + ApiRoutes.Apartments.GetAll;
            if (urlParts.Any())
            {
                url += "?" + string.Join("&", urlParts);
            }
            return new Uri(url);
        }

        public Uri GetUserUri(string userId)
        {
            return new Uri(baseUri + ApiRoutes.Users.Get.Replace("{userId}", userId));
        }

        public Uri GetUsersUri(PaginationFilter filter)
        {
            var urlParts = new List<string>();
            if (filter.Limit.HasValue) urlParts.Add($"limit={filter.Limit.Value}");
            if (filter.Offset.HasValue) urlParts.Add($"offset={filter.Offset.Value}");

            var url = baseUri + ApiRoutes.Users.GetAll;
            if (urlParts.Any())
            {
                url += "?" + string.Join("&", urlParts);
            }
            return new Uri(url);
        }

        public Uri GetUserProfileImageUri(string userId)
        {
            return new Uri(baseUri + ApiRoutes.Users.GetProfileImage.Replace("{userId}", userId));
        }

        public Uri GetEmailConfirmationUri(string userId, string token)
        {
            return new Uri(emailConfirmationLinkBase + "?userId=" + userId + "&token=" + token);
        }

        public Uri GetPasswordResetUri(string userId, string token)
        {
            return new Uri(passwordResetLinkBase + "?userId=" + userId + "&token=" + token);
        }

        public Uri GetInvitationEmailUri()
        {
            return new Uri(invitationLinkBase);
        }
    }
}
