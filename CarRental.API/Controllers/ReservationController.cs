using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private IReservationRepository _reservationRepository;
        private ICustomerRepository _customerRepository;
        private IVehicleRepository _vehicleRepository;
        private IMapper _mapper;

        public ReservationController(IReservationRepository reservationRepo, ICustomerRepository customerRepo, IVehicleRepository vehicleRepo, IMapper mapper)
        {
            _reservationRepository = reservationRepo;
            _customerRepository = customerRepo;
            _vehicleRepository = vehicleRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetreservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservationAsync(ReservationDTO reservationDTO)
        {
            var mapReservation = _mapper.Map<Reservation>(reservationDTO);
            var customerId = await _customerRepository.GetCustomerByIdAsync(mapReservation.CustomerID);
            var vehicleId = await _vehicleRepository.GetVehicleByIdAsync(mapReservation.VehicleID);
            var allReservations = await _reservationRepository.GetAllReservationsAsync();
            var reservedVehicle = allReservations.FirstOrDefault(v => v.VehicleID == mapReservation.VehicleID);

            if (customerId == null || vehicleId == null)
            {
                return NotFound();
            }
            if (reservedVehicle != null)
            {
                return BadRequest("The vehicle is already reserved.");
            }
            if (mapReservation.DateFrom > mapReservation.DateTo || mapReservation.DateFrom < DateTime.Now || mapReservation.DateTo < DateTime.Now)
            {
                return BadRequest("The date range for vehicle reservation is incorrect.");
            }
            if (allReservations.FindAll(r => r.CustomerID == mapReservation.CustomerID).Count() > 3)
            {
                return BadRequest("The customer cannot have more than 3 reservations.");
            }
            if ((mapReservation.DateTo - mapReservation.DateFrom) < TimeSpan.FromDays(3))
            {
                return BadRequest("Vehicle reservation time cannot be less than 3 days.");
            }

            var newReservation = await _reservationRepository.AddReservationAsync(mapReservation);
            return Ok(newReservation);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReservationAsync(int id, ReservationDTO reservationDTO)
        {
            var mapReservation = _mapper.Map<Reservation>(reservationDTO);
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (mapReservation.DateTo < reservation.DateTo.AddDays(1))
            {
                return BadRequest("The date cannot be changed for less than 1 day.");
            }
            var updateReservation = await _reservationRepository.UpdateReservationAsync(id, mapReservation);
            return Ok(updateReservation);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            await _reservationRepository.DeleteReservationAsync(id);
            return Ok();
        }

    }
}
