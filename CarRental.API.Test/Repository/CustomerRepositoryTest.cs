using CarRental.API.Entities;
using CarRental.API.Models;
using CarRental.API.Repositories;

namespace CarRental.API.Test.Repository
{
    public class CustomerRepositoryTest
    {
        [Fact]
        public async Task GetCustomersAsync_NewlyIntializedStaticListWithTwoUsers_returnsTwoUsers()
        {
            //Arrange
            CustomerRepository.Customers.Clear();
            CustomerRepository.Customers =
            [
                new Customer
                {
                    CustomerID = 1,
                    FirstName = "Ken",
                    LastName = "Tamburello",
                    Phone = "0897-012345",
                    Address = null,
                    User =  new User
                    {
                        UserID = 1,
                        Email = "Ken.Tamburello@gmail.com",
                        Password = "CCKOT"
                    }
                },
                new Customer
                {
                    CustomerID = 2,
                    FirstName = "Terri",
                    LastName = "Walters",
                    Phone = "(91) 567 8901",
                    Address = new Address
                    {
                        AddressID = 2,
                        Country = "Spain",
                        City ="Madrid",
                        Street = "Gran Vía, 4567",
                        PostalCode= "10071"
                    },
                    User =  new User
                    {
                        UserID = 2,
                        Email = "Terri.Walters@gmail.com",
                        Password = "SIUIH"
                    }
                }
            ];

            var repository = new CustomerRepository();

            //Act
            List<Customer> result = await repository.GetCustomersAsync();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Ken", result[0].FirstName);
            Assert.Equal("Tamburello", result[0].LastName);
            Assert.Null(result[0].Address);
            Assert.NotNull(result[0].User);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShoudBeReturnCustomer_True()
        {
            //Arrange
            CustomerRepository.Customers.Clear();
            CustomerRepository.Customers =
            [
                new Customer
                {
                    CustomerID = 1,
                    FirstName = "Ken",
                    LastName = "Tamburello",
                    Phone = "0897-012345",
                    Address = new Address
                    {
                        AddressID = 1,
                        Country = "Switzerland",
                        City ="Genève",
                        Street = "Grenzacherweg 0123",
                        PostalCode= "10122"
                    },
                    User =  new User
                    {
                        UserID = 1,
                        Email = "Ken.Tamburello@gmail.com",
                        Password = "CCKOT"
                    }
                }
            ];

            //var repositoryMock = new Mock<ICustomerRepository>(); +using Moq
            var repository = new CustomerRepository();

            //Act
            Customer result = await repository.GetCustomerByIdAsync(1);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShoudUpdateCustomer_ReturnNotNull()
        {
            //Arrange
            CustomerRepository.Customers.Clear();
            CustomerRepository.Customers =
            [
                new Customer
                {
                    CustomerID = 1,
                    FirstName = "Ken",
                    LastName = "Tamburello",
                    Phone = "0897-012345",
                    Address = new Address
                    {
                        AddressID = 1,
                        Country = "Switzerland",
                        City ="Genève",
                        Street = "Grenzacherweg 0123",
                        PostalCode= "10122"
                    },
                    User =  new User
                    {
                        UserID = 1,
                        Email = "Ken.Tamburello@gmail.com",
                        Password = "CCKOT"
                    }
                }
            ];

            var updateCustomer = new Customer
            {
                CustomerID = 1,
                FirstName = "Terri",
                LastName = "Walters",
                Phone = "(91) 567 8901",
                Address = new Address
                {
                    AddressID = 1,
                    Country = "Switzerland",
                    City = "Genève",
                    Street = "Grenzacherweg 0123",
                    PostalCode = "10122"
                },
                User = new User
                {
                    UserID = 1,
                    Email = "Ken.Tamburello@gmail.com",
                    Password = "CCKOT"
                }
            };
            var repository = new CustomerRepository();

            //Act
            Customer result = await repository.UpdateCustomerAsync(1, updateCustomer);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerID);
            Assert.Equal("Terri", result.FirstName);
            Assert.Equal(updateCustomer.LastName, result.LastName);
            Assert.Equal(CustomerRepository.Customers[0].CustomerID, result.CustomerID);
        }

        [Fact]
        public async Task UpdateCustomerAsync_NotExistCustomer_()
        {
            //Arrange
            CustomerRepository.Customers.Clear();
            CustomerRepository.Customers =
            [
                new Customer
                {
                    CustomerID = 1,
                    FirstName = "Ken",
                    LastName = "Tamburello",
                    Phone = "0897-012345",
                    Address = new Address
                    {
                        AddressID = 1,
                        Country = "Switzerland",
                        City ="Genève",
                        Street = "Grenzacherweg 0123",
                        PostalCode= "10122"
                    },
                    User =  new User
                    {
                        UserID = 1,
                        Email = "Ken.Tamburello@gmail.com",
                        Password = "CCKOT"
                    }
                }
            ];

            var updateCustomer = new Customer
            {
                CustomerID = 1,
                FirstName = "Terri",
                LastName = "Walters",
                Phone = "(91) 567 8901",
                Address = new Address
                {
                    AddressID = 1,
                    Country = "Switzerland",
                    City = "Genève",
                    Street = "Grenzacherweg 0123",
                    PostalCode = "10122"
                },
                User = new User
                {
                    UserID = 1,
                    Email = "Ken.Tamburello@gmail.com",
                    Password = "CCKOT"
                }
            };
            var repository = new CustomerRepository();

            //Act

            //Assert
            //Assert.ThrowsAsync<ArgumentException>(() => repository.UpdateCustomerAsync(10, updateCustomer));
        }
        [Fact]
        public async Task DeleteCustomerAsync_ShoudBeRemoveCustomer_ReturnNull()
        {
            //Arrange
            CustomerRepository.Customers.Clear();
            CustomerRepository.Customers =
            [
                new Customer
                {
                    CustomerID = 1,
                    FirstName = "Ken",
                    LastName = "Tamburello",
                    Phone = "0897-012345",
                    Address = new Address
                    {
                        AddressID = 1,
                        Country = "Switzerland",
                        City ="Genève",
                        Street = "Grenzacherweg 0123",
                        PostalCode= "10122"
                    },
                    User =  new User
                    {
                        UserID = 1,
                        Email = "Ken.Tamburello@gmail.com",
                        Password = "CCKOT"
                    }
                }
            ];
            var repository = new CustomerRepository();
            //Act
            await repository.DeleteCustomerAsync(1);
            var result = await repository.GetCustomerByIdAsync(1);

            //Assert
            Assert.Null(result);
        }
    }
}
