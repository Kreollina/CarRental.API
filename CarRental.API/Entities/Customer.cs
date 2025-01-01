using CarRental.API.Entities;

namespace CarRental.API.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public User User { get; set; }
    }
}
