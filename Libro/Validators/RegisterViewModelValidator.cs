using FluentValidation;
using Libro.ViewModel;

namespace Libro.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(r => r.Fullname)
                .NotEmpty().WithMessage("Full name is required");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Enter a valid email address");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(r => r.ConfirmPassword)
                .Equal(r => r.Password).WithMessage("Passwords do not match");

            RuleFor(r => r.Role)
                .NotEmpty().WithMessage("Please select a role");
            RuleFor(r => r.ImageUrl)
                .Must(BeAValidUrl).WithMessage("Enter a valid image URL (must start with http:// or https://)")
                .When(r => !string.IsNullOrWhiteSpace(r.ImageUrl));

        }
        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}