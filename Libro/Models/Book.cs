namespace Libro.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Isbn { get; set; }
        public int PublishedYear { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public BookStatus Status { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public decimal BorrowPrice { get; set; }
        public string? Description { get; set; }
        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public string? ImageUrl { get; set; }

        public bool CanBeBorrowed => Status == BookStatus.Available && AvailableCopies > 0;

        public string DisplayStatusForMember =>
            Status == BookStatus.ComingSoon ? "Coming Soon" :
            CanBeBorrowed ? "Available" : "Unavailable";
    }
}
