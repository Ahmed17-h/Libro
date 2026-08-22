using FluentValidation;
using Libro.Models;

namespace Libro.Validators
{
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty().WithMessage("Author name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
            RuleFor(a => a.Name)
    .NotEmpty().WithMessage("Author name is required")
    .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(a => a.Bio)
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters");

            RuleFor(a => a.DateOfBirth)
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past")
                .When(a => a.DateOfBirth.HasValue);

            RuleFor(a => a.WebsiteUrl)
                .Must(BeAValidUrl).WithMessage("Enter a valid URL (must start with http:// or https://)")
                .When(a => !string.IsNullOrWhiteSpace(a.WebsiteUrl));


    
        }
        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}