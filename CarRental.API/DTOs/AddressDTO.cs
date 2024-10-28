namespace CarRental.Api.DTOs
{
    public class AddressDTO
    {
        public int AddressID { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}
