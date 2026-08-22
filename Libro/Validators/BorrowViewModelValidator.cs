using FluentValidation;
using Libro.ViewModel;

namespace Libro.Validators
{
    public class BorrowViewModelValidator : AbstractValidator<BorrowViewModel>
    {
        public BorrowViewModelValidator()
        {
            RuleFor(b => b.DurationInDays)
                .InclusiveBetween(1, 7).WithMessage("Duration must be between 1 and 7 days");
        }
    }
}