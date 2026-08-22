using Libro.Models;

namespace Libro.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByIdWithBooksAsync(int id);
        Task AddAsync(Category category);
        void Remove(Category category);
        Task SaveChangesAsync();
    }
}