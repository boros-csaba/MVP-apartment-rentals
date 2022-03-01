using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResult> LoginAsync(string email, string password);
        Task<UserRegistrationResult> RegisterUserAsync(User user, string password);
        Task<EmailConfirmationResult> ConfirmEmailAsync(string userId, string token);
        Task<Result> ResendConfirmationEmailAsync(User user);
        Task<LoginResult> LoginWithExternalProviderAsync(string provider, string token, string photoUrl);
        Task<Result> UnblockUserAsync(User user);
        Task<Result> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<Result> SendInvitationAsync(string emailAddress);
        Task<LoginResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
