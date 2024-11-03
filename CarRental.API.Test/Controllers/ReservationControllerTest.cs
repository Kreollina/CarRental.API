using AutoMapper;
using CarRental.Api.Controllers;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using CarRental.API.Validation;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.API.Test.Controllers
{
    public class ReservationControllerTest
    {
        [Fact]
        public async Task GetAllReservationsAsync_RepositoryIsMocked_ExpectedDataFromStaticCollection()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedReservationList = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now,
                     DateTo = DateTime.Now.AddHours(8),
                     CustomerID = 1,
                     VehicleID = 1
                 }
            };

            reservationRepository.GetAllReservationsAsync().Returns(expectedReservationList);
            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);
            //Act
            var result = await controller.GetAllReservationsAsync();

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var reservations = okResult.Value as IEnumerable<Reservation>;
            Assert.NotNull(reservations);
            Assert.True(reservations.Any());
            Assert.Equal(1, reservations.ElementAt(0).CustomerID);
            Assert.Equal(1, reservations.ElementAt(0).VehicleID);
        }

        [Fact]
        public async Task GetreservationByIdAsync_RepositoryIsMocked_ExpectedReservationById()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedReservation = new Reservation
            {
                ReservationID = 1,
                Description = "discount 5%",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(8),
                CustomerID = 1,
                VehicleID = 1
            };

            reservationRepository.GetReservationByIdAsync(1).Returns(expectedReservation);
            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);
            //Act
            var result = await controller.GetreservationByIdAsync(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            var reservation = okResult.Value as Reservation;
            Assert.NotNull(reservation);
            Assert.Equal(1, reservation.ReservationID);
        }

        [Fact]
        public async Task GetreservationByIdAsync_RepositoryIsMocked_ExpectedNotFoundReservationById()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();


            reservationRepository.GetReservationByIdAsync(Arg.Any<int>()).Returns((Reservation)null);
            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);
            //Act
            var result = await controller.GetreservationByIdAsync(10);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateReservationAsync_ReservationValidationFailed_ExpectedBadRequestResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();

            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedCustomer = new Customer()
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            var expectedVehicle = new Vehicle()
            {

                VehicleID = 1,
                Producent = "Toyota",
                Model = "RAV4 Hybrid",
                Year = "2022",
                Colour = "White",
                VIN = "TY123456789012345"

            };
            var expectedReservationList = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now,
                     DateTo = DateTime.Now.AddHours(8),
                     CustomerID = 1,
                     VehicleID = 1
                 }
            };
            var reservationDTO = new ReservationDTO
            {
                ReservationID = 1,
                Description = "discount 5%",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(8),
                CustomerID = 1,
                VehicleID = 1
            };

            customerRepository.GetCustomerByIdAsync(reservationDTO.CustomerID).Returns(expectedCustomer);
            vehicleRepository.GetVehicleByIdAsync(reservationDTO.VehicleID).Returns(expectedVehicle);
            reservationRepository.GetAllReservationsAsync().Returns(expectedReservationList);

            validator.Validate(Arg.Any<ValidationContext<ReservationDTO>>()).Returns(new ValidationResult()
            {
                Errors = new List<ValidationFailure>()
                {
                    new ValidationFailure("test", "Test error message")
                }
            });
            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CreateReservationAsync(reservationDTO);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var errorResponse = (ErrorResponse)badRequestResult.Value;
            Assert.Equal(1, errorResponse.Errors.Count);
            Assert.Equal("test", errorResponse.Errors[0].PropertyName);
            Assert.Equal("Test error message", errorResponse.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task CreateReservationAsync_VehicleAlreadyReserved_ExpectedBadRequestResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();

            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedCustomer = new Customer()
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            var expectedVehicle = new Vehicle()
            {

                VehicleID = 1,
                Producent = "Toyota",
                Model = "RAV4 Hybrid",
                Year = "2022",
                Colour = "White",
                VIN = "TY123456789012345"

            };
            var expectedReservationList = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now,
                     DateTo = DateTime.Now.AddHours(8),
                     CustomerID = 1,
                     VehicleID = 1
                 }
            };
            var reservationDTO = new ReservationDTO
            {
                ReservationID = 1,
                Description = "discount 5%",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddHours(8),
                CustomerID = 1,
                VehicleID = 1
            };

            customerRepository.GetCustomerByIdAsync(reservationDTO.CustomerID).Returns(expectedCustomer);
            vehicleRepository.GetVehicleByIdAsync(reservationDTO.VehicleID).Returns(expectedVehicle);
            reservationRepository.GetAllReservationsAsync().Returns(expectedReservationList);
            validator.Validate(Arg.Any<ValidationContext<ReservationDTO>>()).Returns(new ValidationResult());

            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CreateReservationAsync(reservationDTO);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var errorResponse = (string)badRequestResult.Value;
            Assert.Equal("The vehicle is already reserved.", errorResponse);
        }

        [Fact]
        public async Task CreateReservationAsync_MoreThanThreeReservations_ExpectedBadRequestResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();

            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedCustomer = new Customer()
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            var expectedVehicle = new Vehicle()
            {

                VehicleID = 1,
                Producent = "Toyota",
                Model = "RAV4 Hybrid",
                Year = "2022",
                Colour = "White",
                VIN = "TY123456789012345"

            };
            var expectedReservationList = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };
            var reservationDTO = new ReservationDTO
            {
                ReservationID = 6,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 1,
                VehicleID = 4
            };

            customerRepository.GetCustomerByIdAsync(reservationDTO.CustomerID).Returns(expectedCustomer);
            vehicleRepository.GetVehicleByIdAsync(reservationDTO.VehicleID).Returns(expectedVehicle);
            reservationRepository.GetAllReservationsAsync().Returns(expectedReservationList);
            validator.Validate(Arg.Any<ValidationContext<ReservationDTO>>()).Returns(new ValidationResult());

            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CreateReservationAsync(reservationDTO);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var errorResponse = (string)badRequestResult.Value;
            Assert.Equal("The customer cannot have more than 3 reservations.", errorResponse);
        }

        [Fact]
        public async Task CreateReservationAsync_AddReservation_ExpectedOkResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();

            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var expectedCustomer = new Customer()
            {
                CustomerID = 1,
                FirstName = "John",
                LastName = "Doe"
            };
            var expectedVehicle = new Vehicle()
            {

                VehicleID = 1,
                Producent = "Toyota",
                Model = "RAV4 Hybrid",
                Year = "2022",
                Colour = "White",
                VIN = "TY123456789012345"

            };
            var expectedReservationList = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };
            var reservationDTO = new ReservationDTO
            {
                ReservationID = 6,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 3,
                VehicleID = 4
            };
            var mapReservation = new Reservation
            {
                ReservationID = reservationDTO.ReservationID,
                Description = reservationDTO.Description,
                DateFrom = reservationDTO.DateFrom,
                DateTo = reservationDTO.DateTo,
                CustomerID = reservationDTO.CustomerID,
                VehicleID = reservationDTO.VehicleID
            };
            var newReservation = new ReservationDTO
            {
                ReservationID = mapReservation.ReservationID,
                Description = mapReservation.Description,
                DateFrom = mapReservation.DateFrom,
                DateTo = mapReservation.DateTo,
                CustomerID = mapReservation.CustomerID,
                VehicleID = mapReservation.VehicleID
            };

            customerRepository.GetCustomerByIdAsync(reservationDTO.CustomerID).Returns(expectedCustomer);
            vehicleRepository.GetVehicleByIdAsync(reservationDTO.VehicleID).Returns(expectedVehicle);
            reservationRepository.GetAllReservationsAsync().Returns(expectedReservationList);
            reservationRepository.AddReservationAsync(Arg.Any<Reservation>()).Returns(mapReservation);

            validator.Validate(Arg.Any<ValidationContext<ReservationDTO>>()).Returns(new ValidationResult());

            var mapper = Substitute.For<IMapper>();
            mapper.Map<Reservation>(reservationDTO).Returns(mapReservation);
            mapper.Map<ReservationDTO>(mapReservation).Returns(newReservation);

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CreateReservationAsync(reservationDTO);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var reservation = okResult.Value as ReservationDTO;
            Assert.NotNull(reservation);
            Assert.Equal(newReservation.ReservationID, reservation.ReservationID);
            Assert.Equal(newReservation.Description, reservation.Description);
            Assert.Equal(newReservation.DateFrom, reservation.DateFrom);
            Assert.Equal(newReservation.DateTo, reservation.DateTo);
            Assert.Equal(newReservation.CustomerID, reservation.CustomerID);
            Assert.Equal(newReservation.VehicleID, reservation.VehicleID);
        }

        [Fact]
        public async Task UpdateReservationAsync_ReservarvationIsNotExists_ExpectedNotFoundResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var updatedReservation = new ReservationDTO
            {
                ReservationID = 6,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 1,
                VehicleID = 4
            };
            reservationRepository.GetReservationByIdAsync(updatedReservation.ReservationID).Returns((Reservation)null);

            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.UpdateReservationAsync(6, updatedReservation);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateReservationAsync_ReservarvationIsExists_ExpectedOkObjectResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var reservations = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(4),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };

            var reservationDTO = new ReservationDTO
            {
                ReservationID = 4,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(5),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 1,
                VehicleID = 4
            };
            var mapReservation = new Reservation
            {
                ReservationID = reservationDTO.ReservationID,
                Description = reservationDTO.Description,
                DateFrom = reservationDTO.DateFrom,
                DateTo = reservationDTO.DateTo,
                CustomerID = reservationDTO.CustomerID,
                VehicleID = reservationDTO.VehicleID
            };
            reservationRepository.GetReservationByIdAsync(reservationDTO.ReservationID).Returns(reservations[3]);

            var mapper = Substitute.For<IMapper>();
            mapper.Map<Reservation>(reservationDTO).Returns(mapReservation);

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.UpdateReservationAsync(reservations[3].ReservationID, reservationDTO);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateReservationAsync_InvalidDateChange_ExpectedBadRequestResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var reservations = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddHours(5),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };

            var reservationDTO = new ReservationDTO
            {
                ReservationID = 4,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(5),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 1,
                VehicleID = 4
            };
            reservationRepository.GetReservationByIdAsync(reservationDTO.ReservationID).Returns(reservations[3]);

            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.UpdateReservationAsync(reservations[3].ReservationID, reservationDTO);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var errorResponse = (string)badRequestResult.Value;
            Assert.Equal("The date cannot be changed for less than 1 day before.", errorResponse);
        }

        [Fact]
        public async Task CancelReservationAsync_InvalidDate_ExpectedBadRequestResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var reservations = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddHours(5),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };
            reservationRepository.GetReservationByIdAsync(reservations[3].ReservationID).Returns(reservations[3]);
            var mapper = Substitute.For<IMapper>();
            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CancelReservationAsync(reservations[3].ReservationID);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            var errorResponse = (string)badRequestResult.Value;
            Assert.Equal("The reservation cannot be canceled for less than 2 day before.", errorResponse);
        }

        [Fact]
        public async Task CancelReservationAsync_ReservarvationIsNotExists_ExpectedNotFoundResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var cancelReservation = new ReservationDTO
            {
                ReservationID = 6,
                Description = "discount 5%",
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(8),
                CustomerID = 1,
                VehicleID = 4
            };
            reservationRepository.GetReservationByIdAsync(cancelReservation.ReservationID).Returns((Reservation)null);

            var mapper = Substitute.For<IMapper>();

            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CancelReservationAsync(cancelReservation.ReservationID);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CancelReservationAsync_ReservarvationIsExists_ExpectedOkObjectResult()
        {
            //Arrange
            var reservationRepository = Substitute.For<IReservationRepository>();
            var customerRepository = Substitute.For<ICustomerRepository>();
            var vehicleRepository = Substitute.For<IVehicleRepository>();
            var validator = Substitute.For<IValidator<ReservationDTO>>();

            var reservations = new List<Reservation>()
            {
                 new Reservation
                 {
                     ReservationID= 1,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 2,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 2
                 },
                 new Reservation
                 {
                     ReservationID= 3,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 3
                 },
                 new Reservation
                 {
                     ReservationID= 4,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(4),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 1,
                     VehicleID = 1
                 },
                 new Reservation
                 {
                     ReservationID= 5,
                     Description= "discount 5%",
                     DateFrom = DateTime.Now.AddDays(2),
                     DateTo = DateTime.Now.AddDays(8),
                     CustomerID = 2,
                     VehicleID = 1
                 },
            };
            reservationRepository.GetReservationByIdAsync(reservations[3].ReservationID).Returns(reservations[3]);
            var mapper = Substitute.For<IMapper>();
            var controller = new ReservationController(reservationRepository, customerRepository, vehicleRepository, mapper, validator);

            //Act
            var result = await controller.CancelReservationAsync(reservations[3].ReservationID);

            //Assert

            Assert.IsType<OkResult>(result);
        }
    }
}
