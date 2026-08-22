using Libro.Models;

namespace Libro.Repositories
{
    public interface ILibrarianRepository
    {
        Task<Librarian?> GetByIdentityUserIdAsync(string identityUserId);
        Task SaveChangesAsync();
    }
}