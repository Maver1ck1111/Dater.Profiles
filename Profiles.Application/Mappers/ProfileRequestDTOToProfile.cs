using AutoMapper;
using Profiles.Application.DTOs;

namespace Profiles.Application.Mappers
{
    public class ProfileRequestDTOToProfile : Profile
    {
        public ProfileRequestDTOToProfile() 
        {
            CreateMap<ProfileRequestDTO, Profiles.Domain.Profile>()
                .ForMember(dest => dest.ProfileID, opt => opt.Ignore())
                .ForMember(dest => dest.AccountID, opt => opt.MapFrom(src => src.AccountID))
                .ForMember(dest => dest.ImagePaths, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SportInterest, opt => opt.MapFrom(src => src.SportInterest))
                .ForMember(dest => dest.BookInterest, opt => opt.MapFrom(src => src.BookInterest))
                .ForMember(dest => dest.FoodInterest, opt => opt.MapFrom(src => src.FoodInterest))
                .ForMember(dest => dest.LifestyleInterest, opt => opt.MapFrom(src => src.LifestyleInterest))
                .ForMember(dest => dest.MovieInterest, opt => opt.MapFrom(src => src.MovieInterest))
                .ForMember(dest => dest.MusicInterest, opt => opt.MapFrom(src => src.MusicInterest))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.TravelInterest, opt => opt.MapFrom(src => src.TravelInterest))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
                
        }
    }
}