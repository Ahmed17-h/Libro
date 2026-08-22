using Libro.Models;
using Libro.ViewModel;

namespace Libro.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdWithBookAsync(int id);
        Task<List<Loan>> GetActiveLoansForMemberAsync(int memberId);
        Task<List<int>> GetActiveBookIdsForMemberAsync(int memberId);
        Task<List<Loan>> GetLoanHistoryForMemberAsync(int memberId);
        Task<List<Loan>> GetAllActiveLoansAsync();
        Task<int> CountActiveLoansForMemberAsync(int memberId);
        Task<decimal> SumFinesForMemberAsync(int memberId);
        Task<Loan?> GetByIdAsync(int id);
        Task<List<Loan>> GetUnpaidFinesAsync();
        Task<int> CountOverdueLoansAsync();
        Task<decimal> SumUnpaidFinesAsync();
        Task<List<TopBookStat>> GetTopBorrowedBooksAsync(int count);
        Task<List<Loan>> GetAllForExportAsync();
        Task<bool> HasMemberCompletedLoanAsync(int bookId, int memberId);
        Task AddAsync(Loan loan);
        Task SaveChangesAsync();
    }
}