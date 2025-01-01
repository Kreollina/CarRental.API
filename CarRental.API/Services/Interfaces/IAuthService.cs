using CarRental.API.Entities;

namespace CarRental.API.Services.Interfaces
{
    public interface IAuthService
    {
        User Authenticate(string username, string password);
    }
}
