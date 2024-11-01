using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;

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
