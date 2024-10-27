using CarRental.API.Models;

namespace CarRental.API.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        public Task<List<Vehicle>> GetVehiclesAsync();
        public Task<Vehicle> GetVehicleByIdAsync(int id);
        public Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        public Task DeleteVehicleAsync(int id);
    }
}
