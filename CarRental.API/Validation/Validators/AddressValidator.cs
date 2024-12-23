using CarRental.Api.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country can't be empty.")
                .MaximumLength(50).WithMessage("Country name can't exceed 50 characters.")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("Country mast contain only letters.");

            RuleFor(x => x.City).NotEmpty().WithMessage("City can't be empty.")
                .MaximumLength(50).WithMessage("City name can't exceed 50 characters.")
                .Matches("^[a-zA-Z\\s]+$").WithMessage("City mast contain only letters.");

            RuleFor(x => x.Street).NotEmpty().WithMessage("Street can't be empty.")
                .MaximumLength(100).WithMessage("Street name can't exceed 100 characters.")
                .Matches("^[a-zA-Z0-9\\s.,/-]+$").WithMessage("Street must contain only letters, digits, spaces, commas, and periods.");

            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal code cannot be empty.")
                .Matches(@"^(\d{2}-\d{3}|\d{3}-\d{2}|\d{5})$").WithMessage("Postal code format is not correct.");
        }
    }
}
