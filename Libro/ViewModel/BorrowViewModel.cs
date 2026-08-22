namespace Libro.ViewModel
{
    public class BorrowViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public decimal BorrowPricePerDay { get; set; }
        public int DurationInDays { get; set; } = 1;
    }
}