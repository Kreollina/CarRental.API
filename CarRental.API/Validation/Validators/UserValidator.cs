using CarRental.API.DTOs;
using FluentValidation;

namespace CarRental.API.Validation.Validators
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z].*[A-Z]").WithMessage("Password must contain at least 2 uppercase letters.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.RepeatPassword).NotEmpty().WithMessage("Repeat password cannot be empty.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
