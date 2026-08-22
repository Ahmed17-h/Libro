using FluentValidation;
using Libro.Models;

namespace Libro.Validators
{
    public class ReviewValidator : AbstractValidator<Review>
    {
        public ReviewValidator()
        {
            RuleFor(r => r.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars");

            RuleFor(r => r.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
        }
    }
}