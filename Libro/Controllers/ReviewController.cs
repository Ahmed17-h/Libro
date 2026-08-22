using Libro.Models;
using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    [Authorize(Roles = "Member")]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IMemberRepository _memberRepository;

        public ReviewController(
            IReviewRepository reviewRepository,
            ILoanRepository loanRepository,
            IMemberRepository memberRepository)
        {
            _reviewRepository = reviewRepository;
            _loanRepository = loanRepository;
            _memberRepository = memberRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int bookId, int rating, string? comment)
        {
            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            bool hasCompletedLoan = await _loanRepository.HasMemberCompletedLoanAsync(bookId, member.Id);

            if (!hasCompletedLoan)
            {
                TempData["ErrorMessage"] = "You can only review books you've borrowed and returned.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            if (rating < 1 || rating > 5)
            {
                TempData["ErrorMessage"] = "Rating must be between 1 and 5 stars.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var existingReview = await _reviewRepository.GetMemberReviewForBookAsync(bookId, member.Id);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.CreatedDate = DateTime.Now;
                _reviewRepository.Update(existingReview);
            }
            else
            {
                var review = new Review
                {
                    BookId = bookId,
                    MemberId = member.Id,
                    Rating = rating,
                    Comment = comment,
                    CreatedDate = DateTime.Now
                };
                await _reviewRepository.AddAsync(review);
            }

            await _reviewRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thanks for your review!";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        private async Task<Member?> GetCurrentMemberAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (identityUserId == null) return null;
            return await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
        }
    }
}