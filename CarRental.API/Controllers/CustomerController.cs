using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using CarRental.API.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private ICustomerRepository _customerRepository;
        private IMapper _mapper;
        private IValidator<CustomerDTO> _customerValidator;

        public CustomerController(ICustomerRepository customerRepo, IMapper mapper, IValidator<CustomerDTO> customerValidator)
        {
            _customerRepository = customerRepo;
            _mapper = mapper;
            _customerValidator = customerValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetCustomersAsync();
            if (customers == null || !customers.Any())
            {
                return NotFound("No customers found.");
            }
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CustomerDTO customerDTO)
        {
            var context = new ValidationContext<CustomerDTO>(customerDTO);
            var validationResult = _customerValidator.Validate(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            var mapCustomer = _mapper.Map<Customer>(customerDTO);
            var addCustomer = await _customerRepository.AddCustomerAsync(mapCustomer);
            var newCustomer = _mapper.Map<CustomerDTO>(addCustomer);
            return Ok(newCustomer);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomerAsync(int id, CustomerDTO customerDTO)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var context = new ValidationContext<CustomerDTO>(customerDTO);
            var validationResult = _customerValidator.Validate(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            var mapCustomer = _mapper.Map<Customer>(customerDTO);
            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(id, mapCustomer);
            var newCustomerDTO = _mapper.Map<CustomerDTO>(updatedCustomer);
            return Ok(newCustomerDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            await _customerRepository.DeleteCustomerAsync(id);
            return Ok();
        }
    }
}
