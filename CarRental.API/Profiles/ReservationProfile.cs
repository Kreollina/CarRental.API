using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;

namespace CarRental.API.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
        }
    }
}
