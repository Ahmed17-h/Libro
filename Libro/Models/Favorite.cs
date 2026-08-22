namespace Libro.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int MemberId { get; set; }
        public Member? Member { get; set; }

        public DateTime AddedDate { get; set; }
    }
}