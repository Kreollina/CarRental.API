using CarRental.API.Entities;
using CarRental.API.Repositories;
using CarRental.API.Services.Interfaces;

namespace CarRental.API.Services
{
    public class AuthService : IAuthService
    {
        public User Authenticate(string username, string password)
        {
            var user = CustomerRepository.Users.SingleOrDefault(x =>
            string.Equals(x.Email, username, StringComparison.OrdinalIgnoreCase) && x.Password == password);

            return user;
        }
    }
}
