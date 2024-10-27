using CarRental.API.Models;

namespace CarRental.API.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        public Task<List<Reservation>> GetAllReservationsAsync();
        public Task<Reservation> AddReservationAsync(Reservation reservation);
        public Task<Reservation> UpdateReservationAsync(int id, Reservation updateReservation);
        public Task<Reservation> GetReservationByIdAsync(int id);
        public Task DeleteReservationAsync(int id);
    }
}
