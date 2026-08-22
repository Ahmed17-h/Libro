using Libro.Models;

namespace Libro.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync(string? searchTerm, int page, int pageSize);
        Task<int> CountAsync(string? searchTerm);
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByIdNoTrackingAsync(int id);
        Task<Book?> GetByIdWithAuthorAsync(int id);
        Task AddAsync(Book book);
        void Update(Book book);
        void Remove(Book book);
        Task<List<Book>> GetByIdsAsync(List<int> ids);
        Task<bool> HasActiveLoansAsync(int bookId);
        Task<bool> IsIsbnTakenAsync(string isbn, int excludeId);
        Task SaveChangesAsync();
    }
}