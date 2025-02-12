using CarRental.API.DTOs;
using CarRental.API.Models;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class ReservationValidator : AbstractValidator<ReservationDTO>
    {
        public ReservationValidator()
        {
            // Перевірка, що DateTo не є раніше за DateFrom
            RuleFor(x => x.DateTo).GreaterThan(x => x.DateFrom)
                .WithMessage("The end date must be after the start date.");

            // Перевірка, що резервація починається не раніше, ніж за один день від поточного часу
            RuleFor(x => x.DateFrom).Custom((dateFrom, context) =>
            {
                if (context.RootContextData.TryGetValue("OperationType", out var operationType))
                {
                    if (operationType.ToString() == "create" && dateFrom < DateTime.Now.Date.AddDays(1))
                    {
                        context.AddFailure("DateFrom", "Reservations must be made at least 1 day in advance.");
                    }
                    else if (operationType.ToString() == "update" && dateFrom < DateTime.Now.Date.AddDays(1))
                    {
                        context.AddFailure("DateFrom", "The reservation cannot be changed for less than 1 day before.");
                    }
                    else if (operationType.ToString() == "delete" && dateFrom < DateTime.Now.Date.AddDays(2))
                    {
                        context.AddFailure("DateFrom", "The reservation cannot be canceled for less than 2 day before.");
                    }
                }
            });

            // Перевірка, що мінімальна тривалість резервації складає 3 дні (змінити, щоб працювало тільки для методів Update і Create)
            RuleFor(x => x.DateTo).Must((reservationDTO, dateTo) => (dateTo - reservationDTO.DateFrom).TotalDays >= 3)
               .WithMessage("The reservation period must be at least 3 days.");

            // Перевірка, чи не зарезервоване авто у цей період
            RuleFor(x => x.VehicleID).Custom((vehicleID, context) =>
            {
                if (context.RootContextData.TryGetValue("ReservationVehicleConflict", out var conflictType)
                        && context.RootContextData.TryGetValue("ExistingReservationDates", out var dates))
                {
                    if (conflictType.ToString() == "sameCustomer")
                    {
                        context.AddFailure("VehicleID", $"You already reserved this vehicle for the period {dates}.");
                    }
                    else if (conflictType.ToString() == "otherCustomer")
                    {
                        context.AddFailure("VehicleID", $"The vehicle is already reserved by another customer for the period {dates}.");
                    }
                }
            });

            RuleFor(x => x.VehicleID).NotEmpty().WithMessage("Vehicle ID can't be empty.");

            // Перевірка, чи є більше 3 активних резервацій
            RuleFor(x => x).Custom((reservationDTO, context) =>
            {
                if (context.RootContextData.TryGetValue("ExistingReservations", out var existingReservationsObj)
                    && existingReservationsObj is List<Reservation> existingReservations)
                {
                    var customerReservations = existingReservations.Where(r => r.CustomerID == reservationDTO.CustomerID);
                    var overlappingReservations = customerReservations
                        .Count(r => r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom);

                    if (overlappingReservations >= 3)
                    {
                        context.AddFailure("CustomerID", "The customer cannot have more than 3 reservations during the same time period.");
                    }
                }
            });

        }
    }
}
