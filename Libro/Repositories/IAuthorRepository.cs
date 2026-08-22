using Libro.Models;

namespace Libro.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAsync();
        Task<Author?> GetByIdWithBooksAsync(int id);
        Task AddAsync(Author author);
        void Update(Author author);
        void Remove(Author author);
        Task SaveChangesAsync();
    }
}