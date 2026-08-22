namespace Libro.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? IdentityUserId { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Loan>? Loans { get; set; }
        public bool IsSuspended { get; set; }

    }
}
