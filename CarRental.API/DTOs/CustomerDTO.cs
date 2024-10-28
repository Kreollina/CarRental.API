using CarRental.Api.DTOs;

namespace CarRental.API.DTOs
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public AddressDTO Address { get; set; }
        public UserDTO User { get; set; }
    }
}
