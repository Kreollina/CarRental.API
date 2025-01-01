using CarRental.API.Enums;
using Microsoft.AspNetCore.Identity;

namespace CarRental.API.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
