using Libro.Data;
using Libro.Models;
using Microsoft.EntityFrameworkCore;

namespace Libro.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Member>> GetRecentMembersAsync(int count)
        {
            return await _context.Members
                .OrderByDescending(m => m.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Member?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.IdentityUserId == identityUserId);
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<Member?> GetByIdWithLoansAsync(int id)
        {
            return await _context.Members
                .Include(m => m.Loans!)
                    .ThenInclude(l => l.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}