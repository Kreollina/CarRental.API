using CarRental.API.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");
        }
    }
}
