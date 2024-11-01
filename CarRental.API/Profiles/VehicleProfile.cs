using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;

namespace CarRental.API.Profiles
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Vehicle, VehicleDTO>().ReverseMap();
        }
    }
}
