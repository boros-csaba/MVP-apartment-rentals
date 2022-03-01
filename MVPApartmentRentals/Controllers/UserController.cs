using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Contracts;
using MVPApartmentRentals.Helpers;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Services;

namespace MVPApartmentRentals.Controllers
{
    [Authorize]
    public class UserController: Controller
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IUriService uriService;

        public UserController(IMapper mapper, IUserService userService, IUriService uriService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException("mapper");
            this.userService = userService ?? throw new ArgumentNullException("user");
            this.uriService = uriService ?? throw new ArgumentNullException("uriService");
        }

        /// <summary>
        /// Returns all the users in the system
        /// </summary>
        /// <response code="200">Return a list with all the users</response>
        /// <response code="400">Returns a list of validation errors</response>
        [Authorize(Roles="Admin")]
        [HttpGet(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(PaginatedList<UserResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAllAsync([FromQuery] Contracts.PaginationFilter filter)
        {
            var usersFilter = mapper.Map<Models.PaginationFilter>(filter);
            var users = await userService.GetUsersAsync(usersFilter);
            if (!users.Success)
            {
                return BadRequest(new ErrorResponse { Errors = users.Errors });
            }
            var responseList = new List<UserResponse>();
            foreach (var user in users.Data)
            {
                var responseUser = mapper.Map<UserResponse>(user);
                responseUser.ProfileImageLink = GetProfileImageLink(user);
                responseList.Add(responseUser);
            }
            var response = PaginationHelper.CreatePaginatedResponse(responseList, users.TotalCount, usersFilter, f => uriService.GetUsersUri(f).ToString());
            return Ok(response);
        }

        /// <summary>
        /// Returns the user by Id
        /// </summary>
        /// <response code="200">Return the user information</response>
        [HttpGet(ApiRoutes.Users.Get)]
        [ProducesResponseType(typeof(UserResponse), 200)]
        public async Task<IActionResult> Get([FromRoute] string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var responseUser = mapper.Map<UserResponse>(user);
            responseUser.ProfileImageLink = GetProfileImageLink(user);
            return Ok(responseUser);
        }

        /// <summary>
        /// Returns the base64 representation of the user profile image
        /// </summary>
        /// <response code="200">Return the base64 representation of the image</response>
        [HttpGet(ApiRoutes.Users.GetProfileImage)]
        [ProducesResponseType(typeof(ProfileImageResponse), 200)]
        public async Task<IActionResult> GetProfileImage([FromRoute] string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null || user.ProfileImage == null)
            {
                return NotFound();
            }
            return Ok(new ProfileImageResponse
            {
                UserId = userId,
                Base64Image = Convert.ToBase64String(user.ProfileImage)
            });
        }

        /// <summary>
        /// Add a new user to the system
        /// </summary>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">Returns the list of validation errors</response>
        [Authorize(Roles="Admin")]
        [HttpPost(ApiRoutes.Users.Create)]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest user)
        {
            var newUser = mapper.Map<User>(user);
            if (user.Password != user.ConfirmedPassword)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new string[] { "The two passwords do not match!" }
                });
            }
            var result = await userService.CreateUserAsync(newUser, user.Password);
            if (result.Success)
            {
                var responseUser = mapper.Map<UserResponse>(newUser);
                responseUser.ProfileImageLink = GetProfileImageLink(newUser);
                return Created(uriService.GetUserUri(newUser.Id), responseUser);
            }
            else
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors });
            }
        }

        /// <summary>
        /// Update an existing users
        /// </summary>
        /// <response code="200">Returns the updated user</response>
        /// <response code="400">Returns the list of validation errors</response>
        [Authorize(Roles="Admin,Realtor,Client")]
        [HttpPut(ApiRoutes.Users.Update)]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UpdateUserRequest user)
        {
            if (!User.IsInRole("Admin") && ClaimsHelper.GetUserId(User) != userId)
            {
                return Forbid();
            }
            var updatedUser = await userService.GetUserByIdAsync(userId);
            if (updatedUser == null)
            {
                return NotFound();
            }
            var originalRoles = updatedUser.Roles;
            mapper.Map(user, updatedUser);
            if (user.Roles == null)
            {
                updatedUser.Roles = originalRoles;
            }
            var result = await userService.UpdateUserAsync(updatedUser);
            if (result.Success)
            {
                var responseUser = mapper.Map<UserResponse>(result.Data);
                responseUser.ProfileImageLink = GetProfileImageLink(result.Data);
                return Ok(responseUser);
            }
            else
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors });
            }
        }

        /// <summary>
        /// Changes the user profile to the uploaded image
        /// </summary>
        /// <response code="200">Image successfully uploaded</response>
        /// <response code="400">Returns the list of validation errors</response>
        [HttpPost(ApiRoutes.Users.UploadProfileImage)]
        [ProducesResponseType(typeof(ProfileImageResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> UploadProfileImage([FromRoute] string userId, IFormFile image)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != ClaimsHelper.GetUserId(User))
            {
                return Forbid();
            }
            var result = await userService.UpdateProfileImage(user, image.OpenReadStream());
            if (!result.Success)
            {
                return BadRequest(new ErrorResponse { Errors = result.Errors });
            }
            return Ok(new ProfileImageResponse
            {
                UserId = userId,
                Base64Image = Convert.ToBase64String(user.ProfileImage)
            });
        }

        /// <summary>
        /// Delete an existing apartment
        /// </summary>
        /// <response code="204">User is deleted from the system</response>
        /// <response code="400">Returns the list of validation errors</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiRoutes.Users.Delete)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Delete([FromRoute] string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (ClaimsHelper.GetUserId(User) == userId)
            {
                return BadRequest(new ErrorResponse { Errors = new string[] { "You cannot delete yourself!" } });
            }
            var deleted = await userService.DeleteUserAsync(user);
            if (!deleted)
            {
                return BadRequest(new ErrorResponse { Errors = new string[] { "User deletion failed!" } });
            }
            return NoContent();
        }

        private string GetProfileImageLink(User user)
        {
            if (user.ProfileImage == null) return null;
            return uriService.GetUserProfileImageUri(user.Id).ToString();
        }
    }
}
