using Libro.Models;

namespace Libro.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByIdentityUserIdAsync(string identityUserId);
        Task<List<Member>> GetAllAsync();
        Task<Member?> GetByIdWithLoansAsync(int id);
        Task<List<Member>> GetRecentMembersAsync(int count);
        Task SaveChangesAsync();
    }
}