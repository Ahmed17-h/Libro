using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync(string? searchTerm, int page, int pageSize)
        {
            var query = _context.Books.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchTerm) ||
                    (b.Author != null && b.Author.Name.Contains(searchTerm)));
            }

            return await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? searchTerm)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        // نسخة من غير Tracking — مخصصة لحالة الـ Edit عشان منتعارضش
        // مع الكائن الجديد الجاي من الفورم لما نعمل Update عليه بعدين.
        public async Task<Book?> GetByIdNoTrackingAsync(int id)
        {
            return await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetByIdWithAuthorAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

       
        public async Task<List<Book>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Books.Where(b => ids.Contains(b.Id)).ToListAsync();
        }
        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Remove(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<bool> HasActiveLoansAsync(int bookId)
        {
            return await _context.Loans.AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);
        }

        public async Task<bool> IsIsbnTakenAsync(string isbn, int excludeId)
        {
            return await _context.Books.AnyAsync(b => b.Isbn == isbn && b.Id != excludeId);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}