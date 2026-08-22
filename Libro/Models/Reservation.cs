namespace Libro.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public DateTime? ReadyExpiresAt { get; set; }
        public DateTime ReservationDate { get; set; }
        public ReservationStatus Status { get; set; }
    }
}