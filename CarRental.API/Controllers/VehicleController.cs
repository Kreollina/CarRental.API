using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {
        private IVehicleRepository _vehicleRepository;
        private IMapper _mapper;
        public VehicleController(IVehicleRepository vehicleRepo, IMapper mapper)
        {
            _vehicleRepository = vehicleRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicleAsync()
        {
            var vehicles = await _vehicleRepository.GetVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return Ok(vehicle);
        }
        [HttpPost]
        public async Task<IActionResult> CreateVehicleAsync(VehicleDTO vehicleDTO)
        {
            var mapVehicle = _mapper.Map<Vehicle>(vehicleDTO);
            var addVehicle = await _vehicleRepository.AddVehicleAsync(mapVehicle);
            var newVehicle = _mapper.Map<VehicleDTO>(addVehicle);
            return Ok(newVehicle);
        }
        [HttpDelete("{id:int}")]
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