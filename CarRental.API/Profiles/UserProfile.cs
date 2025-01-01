using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Entities;

namespace CarRental.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
