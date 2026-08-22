using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author?> GetByIdWithBooksAsync(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
        }

        public void Remove(Author author)
        {
            _context.Authors.Remove(author);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Update(Author author)
        {
            _context.Authors.Update(author);
        }
    }
}