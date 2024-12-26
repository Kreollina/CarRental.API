using CarRental.API.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class VehicleValidator : AbstractValidator<VehicleDTO>
    {
        public VehicleValidator()
        {
            RuleFor(x => x.Year).
                Matches(@"^\d{4}$").WithMessage("Year must contain only digits, year must be exactly 4 digits.");
        }
    }
}
