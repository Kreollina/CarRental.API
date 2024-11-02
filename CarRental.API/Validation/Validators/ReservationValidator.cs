using CarRental.API.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class ReservationValidator : AbstractValidator<ReservationDTO>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.DateFrom).LessThan(r => r.DateTo).GreaterThanOrEqualTo(DateTime.Now)
                .NotEmpty().WithMessage("The date range for vehicle reservation is incorrect.");
            RuleFor(x => x.DateTo).GreaterThan(x => x.DateFrom + TimeSpan.FromDays(3))
                .NotEmpty().WithMessage("Data wynajęcia nie może być mniejsza niż na 3 dni");
            RuleFor(x => x.CustomerID).NotEmpty();
            RuleFor(x => x.VehicleID).NotEmpty();
            RuleFor(x => x.VehicleID).Custom((x, context) =>
            {
                if (context.RootContextData.ContainsKey("ActualVehicle"))
                {
                    var actualVehicle = context.RootContextData["ActualVehicle"];
                    if (actualVehicle != null)
                        context.AddFailure("Pojazd już jest zarezerwowany");
                }
            });
        }
    }
}
