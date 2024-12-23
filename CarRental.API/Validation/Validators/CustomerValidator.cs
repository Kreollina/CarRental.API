using CarRental.API.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please specify a first name.")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("First name must contain only letters.");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please specify a last name.")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters.")
                .Matches("^[a-zA-Z]+$").WithMessage("Last name must contain only letters.");

            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number can't be empty.")
                .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Invalid phone number format. It must have 10-15 digits and may start with '+'.");

            RuleFor(x => x.Address).SetValidator(new AddressValidator());

            RuleFor(x => x.User).SetValidator(new UserValidator());
        }
    }
}
