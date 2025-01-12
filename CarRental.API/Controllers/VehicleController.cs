using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using CarRental.API.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {
        private IVehicleRepository _vehicleRepository;
        private IMapper _mapper;
        private IValidator<VehicleDTO> _vehicleValidator;

        public VehicleController(IVehicleRepository vehicleRepo, IMapper mapper, IValidator<VehicleDTO> vehicleValidator)
        {
            _vehicleRepository = vehicleRepo;
            _mapper = mapper;
            _vehicleValidator = vehicleValidator;
        }

        [HttpGet("Vehicles")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> GetAllVehicleAsync()
        {
            var vehicles = await _vehicleRepository.GetVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("Vehicle{id:int}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return Ok(vehicle);
        }

        [HttpPost("NewVehicle")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddVehicleAsync(VehicleDTO vehicleDTO)
        {
            var context = new ValidationContext<VehicleDTO>(vehicleDTO);
            var validationResult = await _vehicleValidator.ValidateAsync(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            var mapVehicle = _mapper.Map<Vehicle>(vehicleDTO);
            var addVehicle = await _vehicleRepository.AddVehicleAsync(mapVehicle);
            var newVehicle = _mapper.Map<VehicleDTO>(addVehicle);
            return Ok(newVehicle);
        }

        [HttpDelete("RemoveVehicle{id:int}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            await _vehicleRepository.DeleteVehicleAsync(id);
            return Ok();
        }
    }
}