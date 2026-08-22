using FluentValidation;
using Libro.Data;
using Libro.Models;

namespace Libro.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        private readonly ApplicationDbContext _context;

        public BookValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(b => b.Isbn)
                .NotEmpty().WithMessage("ISBN is required")
                .Must(BeUniqueIsbn).WithMessage("This ISBN is already used by another book");

            RuleFor(b => b.PublishedYear)
                .InclusiveBetween(1000, 2100).WithMessage("Enter a valid year");

            RuleFor(b => b.AuthorId)
                .GreaterThan(0).WithMessage("Please select an author");

            RuleFor(b => b.TotalCopies)
                .GreaterThanOrEqualTo(1).WithMessage("Total copies must be at least 1");

            RuleFor(b => b.BorrowPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");
            RuleFor(b => b.ImageUrl)
                .Must(BeAValidUrl).WithMessage("Enter a valid image URL (must start with http:// or https://)")
                .When(b => !string.IsNullOrWhiteSpace(b.ImageUrl));

        }

        private bool BeUniqueIsbn(Book book, string isbn)
        {
            return !_context.Books.Any(b => b.Isbn == isbn && b.Id != book.Id);
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}