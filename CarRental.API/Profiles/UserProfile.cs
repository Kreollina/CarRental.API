using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Entities;
using CarRental.API.Profiles.Resolvers;

namespace CarRental.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>().ForMember(dest => dest.Password, opt => opt.MapFrom<PasswordResolver>());
            CreateMap<UserDTO, User>();
        }
    }
}
