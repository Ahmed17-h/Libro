using Libro.Models;

namespace Libro.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetReviewsForBookAsync(int bookId);
        Task<Review?> GetMemberReviewForBookAsync(int bookId, int memberId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<int> GetReviewCountAsync(int bookId);
        Task AddAsync(Review review);
        void Update(Review review);
        Task SaveChangesAsync();
    }
}