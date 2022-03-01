using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles="Client,Realtor,Admin")]
    public class ApartmentController: Controller
    {
        private readonly IApartmentService apartmentService;
        private readonly IMapper mapper;
        private readonly IUriService uriService;
        private readonly IUserService userService;

        public ApartmentController(IApartmentService apartmentService, IMapper mapper, IUriService uriService, IUserService userService)
        {
            this.apartmentService = apartmentService ?? throw new ArgumentNullException("apartmentService");
            this.mapper = mapper ?? throw new ArgumentNullException("mapper");
            this.uriService = uriService ?? throw new ArgumentNullException("uriService");
            this.userService = userService ?? throw new ArgumentNullException("userService");
        }

        /// <summary>
        /// Returns all the apartments in the system
        /// </summary>
        /// <response code="200">Return all the available apartments</response>
        /// <response code="400">Returns a list of errors</response>s
        [HttpGet(ApiRoutes.Apartments.GetAll)]
        [ProducesResponseType(typeof(PaginatedResponse<ApartmentResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAllAsync([FromQuery] Contracts.ApartmentFilter filter)
        {
            if (!User.IsInRole("Admin")) filter.OnlyAvailable = true;
            var apartmentsFilter = mapper.Map<Models.ApartmentFilter>(filter);
            var apartments = await apartmentService.GetApartmentsAsync(apartmentsFilter);
            if (!apartments.Success)
            {
                return BadRequest(new ErrorResponse { Errors = apartments.Errors });
            }
            var responseList = mapper.Map<List<ApartmentResponse>>(apartments.Data);
            var response = PaginationHelper.CreatePaginatedResponse(responseList, apartments.TotalCount, apartmentsFilter, f => uriService.GetApartmentsUri(f).ToString());
            return Ok(response);
        }

        /// <summary>
        /// Returns all the apartments owned by the user
        /// </summary>
        /// <response code="200">Return all the apartments that the user owns</response>
        /// <response code="400">Returns a list of errors</response>
        [Authorize(Roles = "Admin,Realtor")]
        [HttpGet(ApiRoutes.Apartments.GetUserApartments)]
        [ProducesResponseType(typeof(PaginatedResponse<ApartmentResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAllForUserAsync([FromRoute] string userId, [FromQuery] Contracts.PaginationFilter filter)
        {
            if (!User.IsInRole("Admin") && ClaimsHelper.GetUserId(User) != userId)
            {
                return Forbid();
            }
            var apartmentsFilter = mapper.Map<Models.ApartmentFilter>(filter);
            apartmentsFilter.UserId = userId;
            var apartments = await apartmentService.GetApartmentsAsync(apartmentsFilter);
            if (!apartments.Success)
            {
                return BadRequest(new ErrorResponse { Errors = apartments.Errors });
            }
            var responseList = mapper.Map<List<ApartmentResponse>>(apartments.Data);
            var response = PaginationHelper.CreatePaginatedResponse(responseList, apartments.TotalCount, apartmentsFilter, f => uriService.GetApartmentsUri(f).ToString());
            return Ok(response);
        }

        /// <summary>
        /// Returns an apartments by Id
        /// </summary>
        /// <response code="200">Returns the apartment</response>
        /// <response code="404">Apartment not found</response>
        [HttpGet(ApiRoutes.Apartments.Get)]
        [ProducesResponseType(typeof(ApartmentResponse), 200)]
        public async Task<IActionResult> Get([FromRoute] string apartmentId)
        {
            var apartment = await apartmentService.GetApartmentByIdAsync(apartmentId);
            if (apartment == null) return NotFound();
            return Ok(mapper.Map<ApartmentResponse>(apartment));
        }

        /// <summary>
        /// Adds a new apartment to the system
        /// </summary>
        /// <response code="201">Returns the newly created apartment</response>
        /// <response code="400">Returns the list of validation errors</response>
        [Authorize(Roles = "Admin,Realtor")]
        [HttpPost(ApiRoutes.Apartments.Create)]
        [ProducesResponseType(typeof(ApartmentResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateApartmentRequest apartment)
        {
            if (!User.IsInRole("Admin") && ClaimsHelper.GetUserId(User) != apartment.RealtorUserId)
            {
                return Forbid();
            }
            var newApartment = mapper.Map<Apartment>(apartment);
            newApartment.AddedDate = DateTime.Now;
            newApartment.Realtor = await userService.GetUserByIdAsync(apartment.RealtorUserId);
            var result = await apartmentService.CreateApartmentAsync(newApartment);
            if (result.Success)
            {
                return Created(uriService.GetApartmentUri(newApartment.Id), mapper.Map<ApartmentResponse>(result.Data));
            }
            return BadRequest(new ErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Update an existing apartment
        /// </summary>
        /// <response code="200">Returns the updated apartment</response>
        /// <response code="400">Returns a list of validation errors</response>
        [Authorize(Roles = "Admin,Realtor")]
        [HttpPut(ApiRoutes.Apartments.Update)]
        [ProducesResponseType(typeof(ApartmentResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Update([FromRoute] string apartmentId, [FromBody] UpdateApartmentRequest apartment)
        {
            var updatedApartment = await apartmentService.GetApartmentByIdAsync(apartmentId);
            if (updatedApartment == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Admin") && ClaimsHelper.GetUserId(User) != apartment.RealtorUserId)
            {
                return Forbid();
            }
            
            mapper.Map(apartment, updatedApartment);
            updatedApartment.Realtor = await userService.GetUserByIdAsync(apartment.RealtorUserId);
            var result = await apartmentService.UpdateApartmentAsync(updatedApartment);
            if (result.Success)
            {
                return Ok(mapper.Map<ApartmentResponse>(result.Data));
            }
            return BadRequest(new ErrorResponse { Errors = result.Errors });
        }

        /// <summary>
        /// Delete an existing apartment
        /// </summary>
        /// <response code="204">Apartment is deleted from the system</response>
        /// <response code="400">Returns the list of validation errors</response>
        [Authorize(Roles = "Admin,Realtor")]
        [HttpDelete(ApiRoutes.Apartments.Delete)]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Delete([FromRoute] string apartmentId)
        {
            var apartment = await apartmentService.GetApartmentByIdAsync(apartmentId);
            if (apartment == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("Admin") && ClaimsHelper.GetUserId(User) != apartment.Realtor.Id)
            {
                return Forbid();
            }
            var deleted = await apartmentService.DeleteApartmentAsync(apartment);
            if (!deleted)
            {
                return BadRequest(new ErrorResponse { Errors = new string[] { "Apartment deletion failed!" } });
            }
            return NoContent();
        }
    }
}
