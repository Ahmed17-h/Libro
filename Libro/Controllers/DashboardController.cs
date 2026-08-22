using Libro.Repositories;
using Libro.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Libro.Controllers
{
    [Authorize(Roles = "Librarian")]
    public class DashboardController : Controller
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IBookRepository _bookRepository;

        public DashboardController(
            ILoanRepository loanRepository,
            IMemberRepository memberRepository,
            IBookRepository bookRepository)
        {
            _loanRepository = loanRepository;
            _memberRepository = memberRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalBooks = await _bookRepository.CountAsync(null),
                TotalMembers = (await _memberRepository.GetAllAsync()).Count,
                OverdueLoansCount = await _loanRepository.CountOverdueLoansAsync(),
                UnpaidFinesTotal = await _loanRepository.SumUnpaidFinesAsync(),
                TopBorrowedBooks = await _loanRepository.GetTopBorrowedBooksAsync(5),
                RecentMembers = await _memberRepository.GetRecentMembersAsync(5)
            };

            return View(model);
        }
        public async Task<IActionResult> ExportLoansReport()
        {
            var loans = await _loanRepository.GetAllForExportAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Book,Member,BorrowDate,DueDate,ReturnDate,Cost,Fine,FinePaid");

            foreach (var loan in loans)
            {
                csv.AppendLine(string.Join(",",
                    EscapeCsv(loan.Book?.Title),
                    EscapeCsv(loan.Member?.FullName),
                    loan.BorrowDate.ToString("yyyy-MM-dd"),
                    loan.DueDate.ToString("yyyy-MM-dd"),
                    loan.ReturnDate?.ToString("yyyy-MM-dd") ?? "",
                    loan.BorrowCost.ToString("0.00"),
                    loan.Fine?.ToString("0.00") ?? "",
                    loan.Fine.HasValue ? loan.IsFinePaid.ToString() : ""
                ));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"loans-report-{DateTime.Now:yyyyMMdd}.csv");
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}