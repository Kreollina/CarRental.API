using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;

namespace CarRental.API.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDTO>().ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<CustomerDTO, Customer>();
        }
    }
}
