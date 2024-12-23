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

        public ReservationController(IReservationRepository reservationRepo, ICustomerRepository customerRepo,
            IVehicleRepository vehicleRepo, IMapper mapper)
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
            // Перевірка, що DateTo не є раніше за DateFrom
            if (reservationDTO.DateTo <= reservationDTO.DateFrom)
            {
                return BadRequest("The end date must be after the start date.");
            }
            // Перевірка, що резервація починається не раніше, ніж за один день від поточного часу
            if (reservationDTO.DateFrom <= DateTime.Now.AddDays(1))
            {
                return BadRequest("Reservations must be made at least 1 day in advance.");
            }
            // Перевірка, що мінімальна тривалість резервації складає 3 дні
            if ((reservationDTO.DateTo - reservationDTO.DateFrom).TotalDays < 3)
            {
                return BadRequest("The reservation period must be at least 3 days.");
            }
            // Отримання всіх існуючих резервацій клієнта            
            var existingReservations = await _reservationRepository.GetAllReservationsAsync();
            // Фільтрація резервацій за клієнтом
            var customerReservations = existingReservations
                .Where(r => r.CustomerID == reservationDTO.CustomerID)
                .ToList();
            // Перевірка, чи є більше 3 активних резервацій у періоді нового резервування
            var overlappingReservations = existingReservations
                .Where(r => r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom)
                .Count();
            if (overlappingReservations >= 3)
            {
                return BadRequest("The customer cannot have more than 3 reservations during the same time period.");
            }
            // Перевірка, чи не зарезервоване авто іншим клієнтом у цей період
            var overlappingVehicleReservations = existingReservations
                .Where(r => r.VehicleID == reservationDTO.VehicleID
                            && r.CustomerID != reservationDTO.CustomerID // інші клієнти
                            && r.DateFrom < reservationDTO.DateTo
                            && r.DateTo > reservationDTO.DateFrom)
                .Any();
            if (overlappingVehicleReservations)
            {
                return BadRequest("The vehicle is already reserved by another customer during the selected period.");
            }
            // Мапимо DTO на модель Reservation
            var reservation = _mapper.Map<Reservation>(reservationDTO);
            // Додаємо резервацію в репозиторій
            var createdReservation = await _reservationRepository.AddReservationAsync(reservation);
            // Мапимо результат на DTO для відповіді
            var resultDTO = _mapper.Map<ReservationDTO>(createdReservation);
            // Повертаємо результат з кодом 200 OK
            return Ok(resultDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReservationAsync(int id, ReservationDTO reservationDTO)
        {
            // Отримуємо існуючу резервацію для оновлення
            var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (existingReservation == null)
            {
                return NotFound("The reservation does not exist.");
            }

            // Перевірка, що DateTo не є раніше за DateFrom
            if (reservationDTO.DateTo <= reservationDTO.DateFrom)
            {
                return BadRequest("The end date must be after the start date.");
            }

            // Перевірка, що резервація починається не раніше, ніж за один день від поточного часу
            if (reservationDTO.DateFrom <= DateTime.Now.AddDays(1))
            {
                return BadRequest("Reservations must be made at least 1 day in advance.");
            }

            // Перевірка, що мінімальна тривалість резервації складає 3 дні
            if ((reservationDTO.DateTo - reservationDTO.DateFrom).TotalDays < 3)
            {
                return BadRequest("The reservation period must be at least 3 days.");
            }

            // Отримання всіх існуючих резервацій, крім тієї, яку ми оновлюємо
            var existingReservations = (await _reservationRepository.GetAllReservationsAsync())
                .Where(r => r.ReservationID != id)
                .ToList();

            // Перевірка кількості активних резервацій клієнта в період нового резервування
            var customerReservations = existingReservations
                .Where(r => r.CustomerID == reservationDTO.CustomerID)
                .ToList();

            var overlappingCustomerReservations = customerReservations
                .Where(r => r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom)
                .Count();

            if (overlappingCustomerReservations >= 3)
            {
                return BadRequest("The customer cannot have more than 3 reservations during the same time period.");
            }

            // Перевірка, чи не зарезервоване авто іншим клієнтом у цей період
            var overlappingVehicleReservations = existingReservations
                .Where(r => r.VehicleID == reservationDTO.VehicleID
                            && r.CustomerID != reservationDTO.CustomerID // інші клієнти
                            && r.DateFrom < reservationDTO.DateTo
                            && r.DateTo > reservationDTO.DateFrom)
                .Any();

            if (overlappingVehicleReservations)
            {
                return BadRequest("The vehicle is already reserved by another customer during the selected period.");
            }

            // Мапимо DTO на модель Reservation
            var updatedReservation = _mapper.Map(reservationDTO, existingReservation);

            // Оновлюємо резервацію в репозиторії
            var result = await _reservationRepository.UpdateReservationAsync(id, updatedReservation);

            // Мапимо результат на DTO для відповіді
            var resultDTO = _mapper.Map<ReservationDTO>(result);

            // Повертаємо результат з кодом 200 OK
            return Ok(resultDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            // Перевірка, що до початку резервації є більше ніж 2 дні
            if (reservation.DateFrom < DateTime.Now.AddDays(2))
            {
                return BadRequest("The reservation cannot be canceled for less than 2 day before.");
            }
            await _reservationRepository.DeleteReservationAsync(id);
            return Ok();
        }

    }
}
