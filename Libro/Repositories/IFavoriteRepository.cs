using Libro.Models;

namespace Libro.Repositories
{
    public interface IFavoriteRepository
    {
        Task<bool> IsFavoriteAsync(int bookId, int memberId);
        Task<List<int>> GetFavoriteBookIdsAsync(int memberId);
        Task<List<Favorite>> GetMyFavoritesAsync(int memberId);
        Task<Favorite?> GetAsync(int bookId, int memberId);
        Task AddAsync(Favorite favorite);
        void Remove(Favorite favorite);
        Task SaveChangesAsync();
    }
}