using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Application.DTOs;
using Whooz_whoo.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Whooz_whoo.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.Ignore()) // Will be resolved from service
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories.Select(c => c.Type.ToString())))
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
                .ForMember(dest => dest.Location,
                    opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.DistanceFromUser,
                    opt => opt.Ignore()) // Calculated at runtime
                .ReverseMap();

            CreateMap<Membership, MembershipDto>()
                .ForMember(dest => dest.Benefits,
                    opt => opt.MapFrom(src => src.Benefits))
                .ForMember(dest => dest.AnnualROI,
                    opt => opt.MapFrom(src => src.CalculateAnnualROI()))
                .ReverseMap();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.CurrentMembership,
                    opt => opt.Ignore()) // Will be loaded separately
                .ForMember(dest => dest.Interests,
                    opt => opt.MapFrom(src => src.Interests.Select(i => i.Category)))
                .ForMember(dest => dest.HomeLocation,
                    opt => opt.MapFrom(src => src.HomeLocation))
                .ReverseMap();

            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<MembershipBenefit, MembershipBenefitDto>().ReverseMap();

            // Value objects
            CreateMap<Location, LocationDto>()
                .ConstructUsing(src => new LocationDto
                {
                    Address = src.Address,
                    City = src.City,
                    Province = src.Province,
                    Country = src.Country,
                    Latitude = src.Latitude,
                    Longitude = src.Longitude
                });

            CreateMap<LocationDto, Location>()
                .ConstructUsing(src => new Location(
                    src.Address,
                    src.City,
                    src.Province,
                    src.Country,
                    src.Latitude,
                    src.Longitude
                ));
        }
    }
}
