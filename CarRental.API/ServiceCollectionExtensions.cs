using CarRental.API.Repositories;
using CarRental.API.Repositories.Interfaces;

namespace CarRental.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCarRentalServices(this IServiceCollection services)
        {
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IVehicleRepository, VehicleRepository>();
            services.AddSingleton<IReservationRepository, ReservationRepository>();
            return services;
        }
    }
}
