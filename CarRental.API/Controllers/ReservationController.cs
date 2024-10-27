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
        public ReservationController(IReservationRepository reservationRepo, ICustomerRepository customerRepo, IVehicleRepository vehicleRepo)
        {
            _reservationRepository = reservationRepo;
            _customerRepository = customerRepo;
            _vehicleRepository = vehicleRepo;
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
        public async Task<IActionResult> CreateReservationAsync(Reservation reservation)
        {
            var customerId = await _customerRepository.GetCustomerByIdAsync(reservation.CustomerID);
            var vehicleId = await _vehicleRepository.GetVehicleByIdAsync(reservation.VehicleID);
            if (customerId == null && vehicleId == null)
            {
                return NotFound();
            }
            _reservationRepository.AddReservationAsync(reservation);
            return Ok(reservation);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReservationAsync(int id, Reservation reservationUpdate)
        {
            var updateReservation = await _reservationRepository.UpdateReservationAsync(id, reservationUpdate);
            if (updateReservation == null)
            {
                return NotFound();
            }
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
