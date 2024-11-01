using AutoMapper;
using CarRental.Api.DTOs;
using CarRental.API.Models;

namespace CarRental.API.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
        }
    }
}
