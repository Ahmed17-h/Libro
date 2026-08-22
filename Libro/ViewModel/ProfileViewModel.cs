namespace Libro.ViewModel
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // للـ Member بس
        public int ActiveLoansCount { get; set; }
        public decimal TotalFines { get; set; }
        public string? ImageUrl { get; set; }
    }
}