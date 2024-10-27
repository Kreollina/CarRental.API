using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;

namespace CarRental.API.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        public static List<Reservation> Reservations = new List<Reservation>()
        {
            new Reservation
            {
                ReservationID= 1,
                Description= "discount 5%",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(8),
                CustomerID = 4, VehicleID = 3
            },
            new Reservation
            {
                ReservationID= 2,
                Description= "discount 5%",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(8),
                CustomerID = 4, VehicleID = 4
            }
        };
        public async Task<Reservation> AddReservationAsync(Reservation reservation)
        {
            reservation.ReservationID = Reservations.Select(r => r.ReservationID).DefaultIfEmpty(0).Max() + 1;
            Reservations.Add(reservation);
            return await Task.FromResult(reservation);
        }

        public async Task DeleteReservationAsync(int id)
        {
            var reservation = await GetReservationByIdAsync(id);
            Reservations.Remove(reservation);
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await Task.FromResult(Reservations);
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            var reservation = Reservations.FirstOrDefault(x => x.ReservationID == id);
            return await Task.FromResult(reservation);
        }

        public async Task<Reservation> UpdateReservationAsync(int id, Reservation updateReservation)
        {
            var reservation = await GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return null;
            }
            reservation.DateFrom = updateReservation.DateFrom;
            reservation.DateTo = updateReservation.DateTo;
            reservation.VehicleID = updateReservation.VehicleID;
            reservation.Description = updateReservation.Description;

            return reservation;
        }
    }
}
