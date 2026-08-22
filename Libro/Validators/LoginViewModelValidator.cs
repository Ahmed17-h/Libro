using FluentValidation;
using Libro.ViewModel;

namespace Libro.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Enter a valid email address");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}