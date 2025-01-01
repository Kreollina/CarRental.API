using AutoMapper;
using CarRental.API.Controllers;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Profiles;
using CarRental.API.Repositories;
using CarRental.API.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace CarRental.API.Test.Controllers
{
    public class CustomerControllerTest
    {
        [Fact]
        public async Task GetAllCustomersAsync_RepositoryNotMocked_ExpectedDataFromStaticCollection()
        {
            //Arrange
            var customerRepository = new CustomerRepository();
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            });

            var mapper = mappingConfig.CreateMapper();

            var customerValidator = Substitute.For<IValidator<CustomerDTO>>();
            var controller = new CustomerController(customerRepository, mapper, customerValidator);

            //Act
            var result = await controller.GetAllCustomersAsync();

            //Assert

            //Assert.True(result.Any());
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var customers = okResult.Value as IEnumerable<Customer>;
            Assert.NotNull(customers);
            Assert.True(customers.Any());
        }

        [Fact]
        public async Task GetAllCustomersAsync_RepositoryIsMocked_ExpectedDataFromStaticCollection()
        {
            //Arrange
            var customerRepository = Substitute.For<ICustomerRepository>();

            var expectedCustomerList = new List<Customer>()
            {
                new Customer()
                {
                    CustomerID = 1,
                    FirstName = "John",
                    LastName = "Doe"
                }
            };
            customerRepository.GetCustomersAsync().Returns(expectedCustomerList);

            var mapper = Substitute.For<IMapper>();

            var customerValidator = Substitute.For<IValidator<CustomerDTO>>();
            var controller = new CustomerController(customerRepository, mapper, customerValidator);

            //Act
            var result = await controller.GetAllCustomersAsync();

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var customers = okResult.Value as IEnumerable<Customer>;
            Assert.NotNull(customers);
            Assert.True(customers.Any());
            Assert.Equal("John", customers.ElementAt(0).FirstName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_RepositoryNotMocked_ExpectedCustomerById()
        {
            //Arrange
            var customerRepository = new CustomerRepository();
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            });

            var mapper = mappingConfig.CreateMapper();
            var customerValidator = Substitute.For<IValidator<CustomerDTO>>();
            var controller = new CustomerController(customerRepository, mapper, customerValidator);

            //Act
            var result = await controller.GetCustomerByIdAsync(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var customer = okResult.Value as Customer;
            Assert.NotNull(customer);
            Assert.Equal(1, customer.CustomerID);

        }

        [Fact]
        public async Task GetCustomerByIdAsync_RepositoryIstMocked_ExpectedCustomerById()
        {
            //Arrange
            var customerRepository = Substitute.For<ICustomerRepository>();
            var expectedCustomer = new Customer()
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            customerRepository.GetCustomerByIdAsync(1).Returns(expectedCustomer);
            var mapper = Substitute.For<IMapper>();

            var customerValidator = Substitute.For<IValidator<CustomerDTO>>();
            var controller = new CustomerController(customerRepository, mapper, customerValidator);

            //Act
            var result = await controller.GetCustomerByIdAsync(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var customer = okResult.Value as Customer;
            Assert.NotNull(customer);
            Assert.Equal(1, customer.CustomerID);
        }

        //[Fact]
        //public async Task ClearCache_CustomerClearCache_WasCalledOnce()
        //{
        //    //Arrange
        //    var customerRepository = Substitute.For<ICustomerRepository>();
        //    var mapper = Substitute.For<IMapper>();
        //
        //    var controller = new CustomerController(customerRepository, mapper);
        //
        //    //Act
        //    await controller.ClearCache();
        //
        //    //Assert
        //    customerRepository.Received(1).ClearCashe();
        //}
    }
}
