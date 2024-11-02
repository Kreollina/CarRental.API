using CarRental.API.Models;

namespace CarRental.API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<List<Customer>> GetCustomersAsync();
        public Task<Customer> GetCustomerByIdAsync(int id);
        public Task<Customer> AddCustomerAsync(Customer customer);
        public Task<Customer> UpdateCustomerAsync(int id, Customer customerNew);
        public Task DeleteCustomerAsync(int id);
        public void ClearCashe();
    }
}
