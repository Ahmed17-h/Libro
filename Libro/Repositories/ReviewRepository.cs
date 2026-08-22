using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsForBookAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.Member)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Review?> GetMemberReviewForBookAsync(int bookId, int memberId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.BookId == bookId && r.MemberId == memberId);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var reviews = await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();

            if (!reviews.Any())
            {
                return 0;
            }

            return reviews.Average(r => r.Rating);
        }

        public async Task<int> GetReviewCountAsync(int bookId)
        {
            return await _context.Reviews.CountAsync(r => r.BookId == bookId);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}