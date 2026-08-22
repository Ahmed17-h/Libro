using Libro.Data;
using Libro.Models;
using Libro.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Loan?> GetByIdWithBookAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Loan>> GetActiveLoansForMemberAsync(int memberId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.MemberId == memberId && l.ReturnDate == null)
                .OrderByDescending(l => l.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<int>> GetActiveBookIdsForMemberAsync(int memberId)
        {
            return await _context.Loans
                .Where(l => l.MemberId == memberId && l.ReturnDate == null)
                .Select(l => l.BookId)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetLoanHistoryForMemberAsync(int memberId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.MemberId == memberId && l.ReturnDate != null)
                .OrderByDescending(l => l.BorrowDate)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetAllActiveLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }

        public async Task<int> CountActiveLoansForMemberAsync(int memberId)
        {
            return await _context.Loans
                .CountAsync(l => l.MemberId == memberId && l.ReturnDate == null);
        }

        public async Task<decimal> SumFinesForMemberAsync(int memberId)
        {
            return await _context.Loans
                .Where(l => l.MemberId == memberId && l.Fine != null)
                .SumAsync(l => l.Fine ?? 0);
        }
        public async Task<Loan?> GetByIdAsync(int id)
        {
            return await _context.Loans.Include(l => l.Member).Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Loan>> GetUnpaidFinesAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.Fine != null && l.Fine > 0 && !l.IsFinePaid)
                .OrderByDescending(l => l.ReturnDate)
                .ToListAsync();
        }
        public async Task<bool> HasMemberCompletedLoanAsync(int bookId, int memberId)
        {
            return await _context.Loans.AnyAsync(l =>
                l.BookId == bookId &&
                l.MemberId == memberId &&
                l.ReturnDate != null);
        }
        public async Task AddAsync(Loan loan)
        {
            await _context.Loans.AddAsync(loan);
        }

        public async Task<int> CountOverdueLoansAsync()
        {
            return await _context.Loans.CountAsync(l =>
                l.ReturnDate == null && l.DueDate.Date < DateTime.Now.Date);
        }

        public async Task<decimal> SumUnpaidFinesAsync()
        {
            return await _context.Loans
                .Where(l => l.Fine != null && !l.IsFinePaid)
                .SumAsync(l => l.Fine ?? 0);
        }

        public async Task<List<TopBookStat>> GetTopBorrowedBooksAsync(int count)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .GroupBy(l => l.Book!.Title)
                .Select(g => new TopBookStat
                {
                    BookTitle = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(s => s.BorrowCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetAllForExportAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.BorrowDate)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}