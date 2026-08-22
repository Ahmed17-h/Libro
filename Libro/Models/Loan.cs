namespace Libro.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        public int MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal? Fine { get; set; }
        public Book? Book { get; set; }
        public Member? Member { get; set; }
        public decimal BorrowCost { get; set; }
        public bool IsFinePaid { get; set; }
        public bool IsRenewed { get; set; }

    }


}



//Id
//BookId (FK)
//Book (Navigation)
//MemberId (FK)
//Member (Navigation)
//BorrowDate
//DueDate
//ReturnDate (nullable — لسه ما رجعش)
//Fine (decimal, nullable أو default 0)