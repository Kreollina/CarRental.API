using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;

namespace CarRental.API.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        public static List<Vehicle> Vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                VehicleID = 1,
                Producent = "Toyota",
                Model = "RAV4 Hybrid",
                Year = "2022",
                Colour = "White",
                VIN = "TY123456789012345"
            },
            new Vehicle
            {
                VehicleID = 2,
                Producent = "Skoda",
                Model = "Octavia A8",
                Year = "2021",
                Colour = "Gray",
                VIN = "SK123456789012345"
            },
            new Vehicle
            {
                VehicleID = 3,
                Producent = "Volkswagen",
                Model = "Jetta VII",
                Year = "2020",
                Colour = "Blue",
                VIN = "VW123456789012345"
            },
            new Vehicle
            {
                VehicleID = 4,
                Producent = "Hyundai",
                Model = "Elantra",
                Year = "2021",
                Colour = "Red",
                VIN = "HY123456789012345"
            },
            new Vehicle
            {
                VehicleID = 5,
                Producent = "BMW",
                Model = "X5 G05",
                Year = "2019",
                Colour = "Black",
                VIN = "BM123456789012345"
            }
        };
        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            vehicle.VehicleID = Vehicles.Select(v => v.VehicleID).DefaultIfEmpty(0).Max() + 1;
            Vehicles.Add(vehicle);
            return await Task.FromResult(vehicle);
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await GetVehicleByIdAsync(id);
            Vehicles.Remove(vehicle);
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.VehicleID == id);
            return await Task.FromResult(vehicle);
        }

        public async Task<List<Vehicle>> GetVehiclesAsync()
        {
            return await Task.FromResult(Vehicles);
        }
    }
}
