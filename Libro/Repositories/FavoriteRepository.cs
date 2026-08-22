using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsFavoriteAsync(int bookId, int memberId)
        {
            return await _context.Favorites.AnyAsync(f => f.BookId == bookId && f.MemberId == memberId);
        }

        public async Task<List<int>> GetFavoriteBookIdsAsync(int memberId)
        {
            return await _context.Favorites
                .Where(f => f.MemberId == memberId)
                .Select(f => f.BookId)
                .ToListAsync();
        }

        public async Task<List<Favorite>> GetMyFavoritesAsync(int memberId)
        {
            return await _context.Favorites
                .Include(f => f.Book)
                .Where(f => f.MemberId == memberId)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync();
        }

        public async Task<Favorite?> GetAsync(int bookId, int memberId)
        {
            return await _context.Favorites.FirstOrDefaultAsync(f => f.BookId == bookId && f.MemberId == memberId);
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public void Remove(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}