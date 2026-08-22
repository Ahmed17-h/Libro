using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation?> GetActiveReservationAsync(int bookId, int memberId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r =>
                r.BookId == bookId &&
                r.MemberId == memberId &&
                (r.Status == ReservationStatus.Waiting || r.Status == ReservationStatus.Ready));
        }

        public async Task<List<Reservation>> GetMyReservationsAsync(int memberId)
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.MemberId == memberId &&
                    (r.Status == ReservationStatus.Waiting || r.Status == ReservationStatus.Ready))
                .OrderBy(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<Reservation?> GetOldestWaitingForBookAsync(int bookId)
        {
            return await _context.Reservations
                .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Waiting)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        
        public async Task<List<Reservation>> GetExpiredReadyReservationsAsync()
        {
            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Ready &&
                            r.ReadyExpiresAt != null &&
                            r.ReadyExpiresAt < DateTime.Now)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}