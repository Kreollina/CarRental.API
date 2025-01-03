﻿using CarRental.API.Entities;
using CarRental.API.Enums;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;

namespace CarRental.API.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public static List<User> Users = new List<User>
        {
            new User
            {
                UserID = 1,
                Email = "admin@mail.net",
                Password = "1111",
                Role = UserRole.Admin
            },
            new User
            {
                UserID = 2,
                Email = "user@mail.net",
                Password = "2222",
                Role = UserRole.User
            },
            new User
            {
                UserID = 3,
                Email = "Roberto.Erickson@gmail.com",
                Password = "3333",
                Role = UserRole.User
            },
            new User
            {
                UserID = 4,
                Email = "Ken.Tamburello@mail.net",
                Password = "12345",
                Role = UserRole.User
            },
            new User
            {
                UserID = 5,
                Email = "Terri.Walters@mail.net",
                Password = "54321",
                Role = UserRole.User
            }
        };

        public static List<Customer> Customers = new List<Customer>
        {
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
                User = Users[3]
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
                User = Users[4]
            },
            new Customer
            {
                CustomerID = 3,
                FirstName = "Roberto",
                LastName = "Erickson",
                Phone = "07-89 01 23",
                Address = new Address
                {
                    AddressID = 3,
                    Country = "Norway",
                    City ="Stavern",
                    Street = "Erling Skakkes gate 2345",
                    PostalCode= "10123"
                },
                User =  Users[2]
            },
            new Customer
            {
                CustomerID = 4,
                FirstName = "Rob",
                LastName = "Goldberg",
                Phone = "(208) 555-0116",
                Address = new Address
                {
                    AddressID = 4,
                    Country = "USA",
                    City ="Boise",
                    Street = "9012 Suffolk Ln.",
                    PostalCode= "10078"
                },
                User =  Users[1]
            },
            new Customer
            {
                CustomerID = 5,
                FirstName = "Gail",
                LastName = "Miller",
                Phone = "(171) 901-2345",
                Address = new Address
                {
                    AddressID = 5,
                    Country = "UK",
                    City ="London",
                    Street = "4567 Wadhurst Rd.",
                    PostalCode= "10088"
                },
                User =  Users[0]
            }
        };

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            customer.CustomerID = Customers.Select(c => c.CustomerID).DefaultIfEmpty(0).Max() + 1;
            customer.Address.AddressID = Customers.Select(c => c.Address.AddressID).DefaultIfEmpty(0).Max() + 1;
            customer.User.UserID = Customers.Select(c => c.User.UserID).DefaultIfEmpty(0).Max() + 1;
            Customers.Add(customer);
            return await Task.FromResult(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await GetCustomerByIdAsync(id);
            Customers.Remove(customer);
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            var customer = Customers.FirstOrDefault(c => c.CustomerID == id);
            return await Task.FromResult(customer);
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await Task.FromResult(Customers);
        }

        public async Task<Customer> UpdateCustomerAsync(int id, Customer updateCustomer)
        {
            var customer = await GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return null;
            }
            customer.FirstName = updateCustomer.FirstName;
            customer.LastName = updateCustomer.LastName;
            customer.Phone = updateCustomer.Phone;

            customer.Address.Country = updateCustomer.Address.Country;
            customer.Address.City = updateCustomer.Address.City;
            customer.Address.Street = updateCustomer.Address.Street;
            customer.Address.PostalCode = updateCustomer.Address.PostalCode;

            customer.User.Password = updateCustomer.User.Password;

            return customer;
        }
    }
}
