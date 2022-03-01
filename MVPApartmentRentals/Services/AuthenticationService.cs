using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Security;

namespace MVPApartmentRentals.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly IKeyProvider keyProvider;
        private readonly DataContext context;
        private readonly IEmailSenderService emailSenderService;
        private readonly IUriService uriService;
        private readonly IEncodingService encodingService;
        private readonly TokenValidationParameters tokenValidationParameters;

        public AuthenticationService(UserManager<User> userManager, IKeyProvider keyProvider, DataContext context, IEmailSenderService emailSenderService, IUriService uriService, IEncodingService encodingService, TokenValidationParameters tokenValidationParameters)
        {
            this.userManager = userManager ?? throw new ArgumentNullException("userManager");
            this.keyProvider = keyProvider ?? throw new ArgumentNullException("keyProvider");
            this.context = context ?? throw new ArgumentNullException("context");
            this.emailSenderService = emailSenderService ?? throw new ArgumentNullException("emailSenderService");
            this.uriService = uriService ?? throw new ArgumentNullException("uriService");
            this.encodingService = encodingService ?? throw new ArgumentNullException("encodingService");
            this.tokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException("tokenValidationParameters");
        }

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null && (user.LockoutEnd != null || user.LockoutEnd > DateTime.Now))
            {
                return new LoginResult { Errors = new string[] { "Your account is blocked, please contact an administrator!" } };
            }
            if (user == null || !await userManager.CheckPasswordAsync(user, password))
            {
                await userManager.AccessFailedAsync(user);
                return new LoginResult { Errors = new string[] { "Email or password is incorrect!" } };
            }
            else
            {
                await userManager.ResetAccessFailedCountAsync(user);
            }

            if (!user.EmailConfirmed)
            {
                return new LoginResult
                {
                    EmailConfirmed = false,
                    UserId = user.Id,
                    Errors = new string[] { "Email address is not confirmed!" }
                };
            }

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<UserRegistrationResult> RegisterUserAsync(User user, string password)
        {
            user.UserName = user.Email;
            if (!IsValidEmail(user.Email))
            {
                return new UserRegistrationResult { Errors = new string[] { "The provided email address is not valid!" } };
            }
            var userCreationResult = await userManager.CreateAsync(user, password);
            if (!userCreationResult.Succeeded)
            {
                
                return new UserRegistrationResult { Errors = userCreationResult.Errors.Select(e => e.Description) };
            }

            await userManager.AddToRoleAsync(user, "Client");
            await SendConfirmationEmailAsync(user);

            return new UserRegistrationResult { UserId = user.Id };
        }

        public async Task<EmailConfirmationResult> ConfirmEmailAsync(string userId, string token)
        {
            var decodedToken = encodingService.Decode(token);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return new EmailConfirmationResult();
                }
                else
                {
                    return new EmailConfirmationResult
                    {
                        Errors = result.Errors.Select(e => e.Description)
                    };
                }
            }
            return new EmailConfirmationResult
            {
                Errors = new string[] { "Invalid User Id or Token!" }
            };
        }

        public async Task<Result> ResendConfirmationEmailAsync(User user)
        {
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    return new Result
                    {
                        Errors = new string[] { "Email address is already confirmed!" }
                    };
                }
                else
                {
                    await SendConfirmationEmailAsync(user);
                    return new Result();
                }
            }
            return new Result
            {
                Errors = new string[] { "Invalid User Id!" }
            };
        }

        public async Task<LoginResult> LoginWithExternalProviderAsync(string provider, string token, string photoUrl)
        {
            if (provider == "Google")
            {
                var information = await GoogleJsonWebSignature.ValidateAsync(token);
                if (information == null)
                {
                    return new LoginResult { Errors = new string[] { "Token is invalid!" } };
                }
                return await LoginUserWithExternalInformation(information.Email, information.GivenName, information.FamilyName, information.Picture);
            }
            else if (provider == "Facebook")
            {
                var webRequest = WebRequest.Create("https://graph.facebook.com/me?fields=id,first_name,email,last_name&access_token=" + token);
                var webRequestResponse = await webRequest.GetResponseAsync();
                if ((webRequestResponse as HttpWebResponse).StatusCode != HttpStatusCode.OK)
                {
                    return new LoginResult { Errors = new string[] { "Token is invalid!" } };
                }
                var resultBody = string.Empty;
                using (Stream responseStream = webRequestResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    resultBody = reader.ReadToEnd();
                }
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultBody);
                return await LoginUserWithExternalInformation(values["email"], values["first_name"], values["last_name"], photoUrl);
            }
            return new LoginResult { Errors = new string[] { "Provider nor supported!" } };
        }

        public async Task<Result> UnblockUserAsync(User user)
        {
            if (user == null)
            {
                return new Result { Errors = new string[] { "User Id is not valid!" } };
            }
            if (user.LockoutEnd == null)
            {
                return new Result { Errors = new string[] { "User is not blocked!" } };
            }
            var result = await userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                return new Result { Errors = result.Errors.Select(e => e.Description) };
            }
            await SendPasswordResetEmailAsync(user);
            return new Result();
        }

        public async Task<Result> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var decodedToken = encodingService.Decode(token);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new Result { Errors = new string[] { "Invalid user id!" } };
            }
            var result = await userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (!result.Succeeded)
            {
                return new Result { Errors = result.Errors.Select(e => e.Description) };
            }
            return new Result();
        }

        public async Task<Result> SendInvitationAsync(string emailAddress)
        {
            var email = new Email
            {
                Subject = "MVP Apartment Rentals invitation",
                ToEmail = emailAddress
            };
            var link = uriService.GetInvitationEmailUri().ToString();
            var body = new StringBuilder();
            body.AppendLine($"<h2>Hi!</h2><br />");
            body.AppendLine("You are invited to join the MVP Apartment Rentals system. Please complete the registration at the following link:<br />");
            body.AppendLine($"<a href=\"{link}\">Register an account</a>");
            email.Content = body.ToString();

            await emailSenderService.SendEmailAsync(email);
            return new Result();
        }

        public async Task<LoginResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var claimsPrincipal = GetPrincipalFromToken(token);
            if (claimsPrincipal != null)
            {
                var jti = claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                var storedRefreshToken = await context.RefreshTokens.Include(r => r.User).SingleOrDefaultAsync(x => x.Token == refreshToken);
                if (storedRefreshToken != null)
                {
                    if (storedRefreshToken.Invalidated)
                    {
                        return new LoginResult { Errors = new string[] { "This refresh token is invalidated!" } };
                    }
                    if (storedRefreshToken.ExpiryDate < DateTime.Now)
                    {
                        return new LoginResult { Errors = new string[] { "This refresh token has expired" } };
                    }
                    if (jti != storedRefreshToken.JwtTokenId)
                    {
                        return new LoginResult { Errors = new string[] { "Invalid tokens!" } };
                    }

                    var result = await GenerateJwtTokenAsync(storedRefreshToken.User);
                    try
                    {
                        context.Remove(storedRefreshToken);
                        await context.SaveChangesAsync();
                    }
                    catch (Exception) { }
                    return result;
                }
            }
            return new LoginResult { Errors = new string[] { "Invalid tokens!" } };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                tokenValidationParameters.ValidateLifetime = true;
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private async Task SendConfirmationEmailAsync(User user)
        {
            var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            confirmationToken = encodingService.Encode(confirmationToken);
            var email = new Email
            {
                Subject = "Confirm your email address",
                ToEmail = user.Email,
                ToName = $"{user.FirstName} {user.LastName}"
            };
            var link = uriService.GetEmailConfirmationUri(user.Id, confirmationToken).ToString();
            var body = new StringBuilder();
            body.AppendLine("<h2>Welcome to MVP Apartment Rentals!</h2>");
            body.AppendLine($"<b>Hi {user.FirstName}!</b><br />");
            body.AppendLine("By clicking the following link you are confirming your email address:<br />");
            body.AppendLine($"<a href=\"{link}\">Confirm email address</a>");
            email.Content = body.ToString();

            await emailSenderService.SendEmailAsync(email);
        }

        private async Task SendPasswordResetEmailAsync(User user)
        {
            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            passwordResetToken = encodingService.Encode(passwordResetToken);
            var email = new Email
            {
                Subject = "Password reset",
                ToEmail = user.Email,
                ToName = $"{user.FirstName} {user.LastName}"
            };
            var link = uriService.GetPasswordResetUri(user.Id, passwordResetToken).ToString();
            var body = new StringBuilder();
            body.AppendLine($"<h2>Hi! {user.FirstName}!</h2><br />");
            body.AppendLine("Your account was unblocked by and administrator. If you forgot your password you can reset it by clicking on the following link:<br />");
            body.AppendLine($"<a href=\"{link}\">Reset password</a>");
            email.Content = body.ToString();

            await emailSenderService.SendEmailAsync(email);
        }

        private async Task<byte[]> GetProfileImageAsync(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    return await webClient.DownloadDataTaskAsync(url);
                }
            }
            catch
            {
                return null;
            }
        }

        private async Task<LoginResult> GenerateJwtTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id)
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenExpiration = DateTime.UtcNow.AddMinutes(30);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiration, 
                SigningCredentials = new SigningCredentials(keyProvider.GetKey(), keyProvider.GetAlgorith()),
                Audience = "Audience",
                Issuer = "Issuer"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshTokenExpiration = DateTime.UtcNow.AddMonths(12);
            var refreshToken = new RefreshToken
            {
                JwtTokenId = token.Id,
                User = user,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = refreshTokenExpiration
            };

            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            return new LoginResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsAdmin = await userManager.IsInRoleAsync(user, "Admin"),
                IsRealtor = await userManager.IsInRoleAsync(user, "Realtor"),
                TokenExpiration = tokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                UserId = user.Id
            };
        }

        private async Task<LoginResult> LoginUserWithExternalInformation(string email, string firstName, string lastName, string photoUrl)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                    ProfileImage = await GetProfileImageAsync(photoUrl)
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return new LoginResult { Errors = result.Errors.Select(e => e.Description) };
                }
            }
            else if (user.ProfileImage == null)
            {
                var profileImage = await GetProfileImageAsync(photoUrl);
                if (profileImage != null)
                {
                    user.ProfileImage = profileImage;
                    await context.SaveChangesAsync();
                }
            }

            await userManager.AddToRoleAsync(user, "Client");
            return await GenerateJwtTokenAsync(user);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
