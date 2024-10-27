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
        public VehicleController(IVehicleRepository vehicleRepo)
        {
            _vehicleRepository = vehicleRepo;
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
        public IActionResult CreateVehicleAsync(Vehicle vehicle)
        {
            _vehicleRepository.AddVehicleAsync(vehicle);
            return Ok(vehicle);
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