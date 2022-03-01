using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Contracts;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Services;

namespace MVPApartmentRentals.Controllers
{
    public class AuthenticationController: Controller
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper, IUserService userService)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException("authenticationService");
            this.mapper = mapper ?? throw new ArgumentNullException("mapper");
            this.userService = userService ?? throw new ArgumentNullException("userService");
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <response code="200">Returns the JWT token and the Refresh token</response>
        /// <response code="400">Login failed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(LoginErrorResponse), 400)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginInformation)
        {
            var login = await authenticationService.LoginAsync(loginInformation.Email, loginInformation.Password);

            if (login.Success)
            {
                var result = mapper.Map<LoginResponse>(login);
                return Ok(result);
            }
            return BadRequest(new LoginErrorResponse
            {
                Errors = login.Errors,
                EmailConfirmed = login.EmailConfirmed,
                UserId = login.UserId
            });
        }

        /// <summary>
        /// Register a new user in the system
        /// </summary>
        /// <response code="200">Returns the ID of the newly created user</response>
        /// <response code="400">User registration failed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.Register)]
        [ProducesResponseType(typeof(UserRegistrationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegistrationRequest registrationInformation)
        {
            var user = mapper.Map<User>(registrationInformation);
            if (registrationInformation.Password != registrationInformation.ConfirmedPassword)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new string[] { "The two passwords do not match!" }
                });
            }

            var registration = await authenticationService.RegisterUserAsync(user, registrationInformation.Password);

            if (registration.Success)
                return Ok(new UserRegistrationResponse { UserId = registration.UserId });

            return BadRequest(new ErrorResponse
            {
                Errors = registration.Errors
            });
        }

        /// <summary>
        /// Confirmes the Email address of the user based on the received token
        /// </summary>
        /// <response code="204">Email successfully confirmed</response>
        /// <response code="400">Email confirmation failed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.ConfirmEmail)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] EmailConfirmationRequest confirmation)
        {
            var result = await authenticationService.ConfirmEmailAsync(confirmation.UserId, confirmation.Token);
            if (result.Success) return NoContent();
            return BadRequest(new ErrorResponse
            {
                Errors = result.Errors
            });
        }

        /// <summary>
        /// Sends the confirmation email again to the user
        /// </summary>
        /// <response code="204">Email was successfully sent</response>
        /// <response code="400">Sending the confirmation Email failed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.ConfirmationEmailRequest)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            var user = await userService.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await authenticationService.ResendConfirmationEmailAsync(user);
            if (result.Success) return NoContent();
            return BadRequest(new ErrorResponse
            {
                Errors = result.Errors
            });
        }

        /// <summary>
        /// Login using the Id token received from Facebook or Google 
        /// </summary>
        /// <response code="200">Returns the JWT token and the Refresh token</response>
        /// <response code="400">Login failed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.ExternalLogin)]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(LoginErrorResponse), 400)]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequest request)
        {
            var login = await authenticationService.LoginWithExternalProviderAsync(request.Provider, request.Token, request.PhotoUrl);

            if (login.Success)
            {
                var response = mapper.Map<LoginResponse>(login);
                return Ok(response);
            }

            return BadRequest(new LoginErrorResponse
            {
                Errors = login.Errors,
                EmailConfirmed = login.EmailConfirmed,
                UserId = login.UserId
            });
        }

        /// <summary>
        /// Removes the account block from a blocked account and send a password reset email to the user
        /// </summary>
        /// <response code="204">User success</response>
        /// <response code="400">User could not be unblocked because of validation errors</response>
        [Authorize(Roles="Admin")]
        [HttpDelete(ApiRoutes.Authentication.Block)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> UnblockUserAsync([FromRoute] string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await authenticationService.UnblockUserAsync(user);
            if (!result.Success)
            {
                return BadRequest(new ErrorResponse() { Errors = result.Errors });
            }
            return NoContent();
        }

        /// <summary>
        /// Reset the password of the user based on the received token
        /// </summary>
        /// <response code="204">Password successfully changed</response>
        /// <response code="400">Password could not be changed because of validation errors</response>
        [HttpPost(ApiRoutes.Authentication.PasswordReset)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
        {
            if (request.Password != request.ConfirmedPassword)
            {
                return BadRequest(new Result
                {
                    Errors = new string[] { "The two passwords do not match!" }
                });
            }
            var result = await authenticationService.ResetPasswordAsync(request.UserId, request.Token, request.Password);
            if (result.Success) return NoContent();
            return BadRequest(new ErrorResponse
            {
                Errors = result.Errors
            });
        }

        /// <summary>
        /// Sends an invitation to the provided email address
        /// </summary>
        /// <response code="204">Email successfully sent.</response>
        /// <response code="400">Email could not be sent.</response>
        [Authorize(Roles="Admin")]
        [HttpPost(ApiRoutes.Authentication.InviteUser)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> SendInvitiation([FromBody] UserInvitationRequest request)
        {
            var result = await authenticationService.SendInvitationAsync(request.Email);
            if (!result.Success)
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors });
            }
            return NoContent();
        }
        
        /// <summary>
        /// Creates a new Jwt Token based on a Refresh Token
        /// </summary>
        /// <response code="200">Return the new token information</response>
        /// <response code="400">Return a list of errors</response>
        [HttpPost(ApiRoutes.Authentication.RefreshToken)]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest request)
        {
            var result = await authenticationService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (result.Success)
            {
                return Ok(mapper.Map<LoginResponse>(result));
            }
            return BadRequest(new ErrorResponse { Errors = result.Errors });
        }


    }
}
