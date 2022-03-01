using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Contracts;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateApartmentRequest, Apartment>()
                .ForMember(a => a.Status, a => a.MapFrom(aa => aa.IsAvailable ? ApartmentStatusEnum.Available : ApartmentStatusEnum.Rented));
            CreateMap<UpdateApartmentRequest, Apartment>()
                .ForMember(a => a.Status, a => a.MapFrom(aa => aa.IsAvailable ? ApartmentStatusEnum.Available : ApartmentStatusEnum.Rented));
            CreateMap<Apartment, ApartmentResponse>()
                .ForMember(a => a.IsAvailable, a => a.MapFrom(aa => aa.Status == ApartmentStatusEnum.Available))
                .ForMember(a => a.RealtorUserId, a => a.MapFrom(aa => aa.Realtor.Id));
            CreateMap<Contracts.ApartmentFilter, Models.ApartmentFilter>();
            CreateMap<LoginResult, LoginResponse>();
            CreateMap<User, UserResponse>()
                .ForMember(u => u.UserId, u => u.MapFrom(uu => uu.Id))
                .ForMember(u => u.IsBlocked, u => u.MapFrom(uu => uu.LockoutEnd != null || uu.LockoutEnd > DateTime.Now));
            CreateMap<Contracts.PaginationFilter, Models.PaginationFilter>();
            CreateMap<Contracts.PaginationFilter, Models.ApartmentFilter>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<UserRegistrationRequest, User>();
        }
    }
}
