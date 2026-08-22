using Libro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libro.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(b => b.BorrowPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.BorrowCost)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.Fine)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books);
        }
        public DbSet<Models.Author> Authors { get; set; }
        public DbSet<Models.Book> Books { get; set; }
        public DbSet<Models.Member> Members { get; set; }
        public DbSet<Models.Loan> Loans { get; set; }
        public DbSet<Models.Librarian> Librarians { get; set; }
        public DbSet<Models.Reservation> Reservations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
