using Libro.Models;

namespace Libro.ViewModel
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int OverdueLoansCount { get; set; }
        public decimal UnpaidFinesTotal { get; set; }
        public List<TopBookStat> TopBorrowedBooks { get; set; } = new();
        public List<Member> RecentMembers { get; set; } = new();
    }

    public class TopBookStat
    {
        public string BookTitle { get; set; } = "";
        public int BorrowCount { get; set; }
    }
}