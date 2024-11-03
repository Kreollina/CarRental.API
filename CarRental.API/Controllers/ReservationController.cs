using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using CarRental.API.Validation;
using FluentValidation;
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
        private IValidator<ReservationDTO> _validator;

        public ReservationController(IReservationRepository reservationRepo, ICustomerRepository customerRepo,
            IVehicleRepository vehicleRepo, IMapper mapper, IValidator<ReservationDTO> validator)
        {
            _reservationRepository = reservationRepo;
            _customerRepository = customerRepo;
            _vehicleRepository = vehicleRepo;
            _mapper = mapper;
            _validator = validator;
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
            var context = new ValidationContext<ReservationDTO>(reservationDTO);

            var customerId = await _customerRepository.GetCustomerByIdAsync(reservationDTO.CustomerID);
            var vehicleId = await _vehicleRepository.GetVehicleByIdAsync(reservationDTO.VehicleID);
            var allReservations = await _reservationRepository.GetAllReservationsAsync();
            var reservedVehicle = allReservations.FirstOrDefault(r => r.VehicleID == reservationDTO.VehicleID
                                                                && r.DateFrom < r.DateTo
                                                                && r.DateTo > reservationDTO.DateFrom);

            context.RootContextData["ActualVehicle"] = reservedVehicle;

            var validationResult = _validator.Validate(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            if (reservedVehicle != null)
            {
                return BadRequest("The vehicle is already reserved.");
            }

            if (allReservations.FindAll(r => r.CustomerID == reservationDTO.CustomerID).Count() > 3)
            {
                return BadRequest("The customer cannot have more than 3 reservations.");
            }
            var mapReservation = _mapper.Map<Reservation>(reservationDTO);
            var reservation = await _reservationRepository.AddReservationAsync(mapReservation);
            var newReservation = _mapper.Map<ReservationDTO>(reservation);

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
            if (reservation.DateFrom < DateTime.Now.AddDays(1))
            {
                return BadRequest("The date cannot be changed for less than 1 day before.");
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
            if (reservation.DateFrom < DateTime.Now.AddDays(2))
            {
                return BadRequest("The reservation cannot be canceled for less than 2 day before.");
            }
            await _reservationRepository.DeleteReservationAsync(id);
            return Ok();
        }

    }
}
