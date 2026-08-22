using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class LibrarianRepository : ILibrarianRepository
    {
        private readonly ApplicationDbContext _context;

        public LibrarianRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Librarian?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Librarians.FirstOrDefaultAsync(l => l.IdentityUserId == identityUserId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}