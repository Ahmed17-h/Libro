using Libro.Models;

namespace Libro.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetActiveReservationAsync(int bookId, int memberId);
        Task<List<Reservation>> GetMyReservationsAsync(int memberId);
        Task<Reservation?> GetOldestWaitingForBookAsync(int bookId);
        Task<Reservation?> GetByIdAsync(int id);
        Task AddAsync(Reservation reservation);
        Task<List<Reservation>> GetExpiredReadyReservationsAsync();
        Task SaveChangesAsync();
    }
}