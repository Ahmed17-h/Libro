using Libro.Models;
using Libro.Repositories;
using Libro.ViewModel;
using Libro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private const int MaxActiveLoansPerMember = 3;
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<LoanController> _logger;
        private readonly ILogger<ReservationController> _reservationLogger;


        public LoanController(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IReservationRepository reservationRepository,
            ILogger<LoanController> logger,
            ILogger<ReservationController> reservationLogger)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _reservationRepository = reservationRepository;
            _logger = logger;
            _reservationLogger = reservationLogger;
        }

        [Authorize(Roles = "Member")]
        [HttpGet]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            var model = new BorrowViewModel
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BorrowPricePerDay = book.BorrowPrice
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            await ExpireOldReservationsAsync();

            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var reservations = await _reservationRepository.GetMyReservationsAsync(member.Id);
            return View(reservations);
        }
        //this for expiring old reservations that are in "Ready" status and have passed their expiration time. It checks for expired reservations, cancels them, and updates the next waiting reservation to "Ready" if applicable.
        private async Task ExpireOldReservationsAsync()
        {
            var expired = await _reservationRepository.GetExpiredReadyReservationsAsync();

            foreach (var reservation in expired)
            {
                reservation.Status = ReservationStatus.Cancelled;
                _logger.LogInformation("Reservation {ReservationId} expired for Book {BookId}", reservation.Id, reservation.BookId);

                var next = await _reservationRepository.GetOldestWaitingForBookAsync(reservation.BookId);
                if (next != null)
                {
                    next.Status = ReservationStatus.Ready;
                    next.ReadyExpiresAt = DateTime.Now.AddHours(48);
                }
            }

            if (expired.Any())
            {
                await _reservationRepository.SaveChangesAsync();
            }
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Borrow(BorrowViewModel model)
        {
            var book = await _bookRepository.GetByIdAsync(model.BookId);

            if (book == null)
            {
                return NotFound();
            }

            if (book.Status != BookStatus.Available || book.AvailableCopies <= 0)
            {
                ModelState.AddModelError("", "This book is not available for borrowing.");
            }

            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            if (member.IsSuspended)
            {
                ModelState.AddModelError("", "Your account has been suspended. Please contact the library.");
            }

            int activeLoansCount = await _loanRepository.CountActiveLoansForMemberAsync(member.Id);

            if (activeLoansCount >= MaxActiveLoansPerMember)
            {
                ModelState.AddModelError("", $"You already have {MaxActiveLoansPerMember} active loans. Return a book before borrowing another.");
            }

            if (!ModelState.IsValid)
            {
                model.BookTitle = book.Title;
                return View(model);
            }

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(model.DurationInDays),
                BorrowCost = book.BorrowPrice * model.DurationInDays
            };

            await _loanRepository.AddAsync(loan);

            book.AvailableCopies -= 1;
            _bookRepository.Update(book);

            await _loanRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Book {BookId} borrowed by Member {MemberId} for {Days} day(s).",
                 book.Id, member.Id, model.DurationInDays);

            TempData["SuccessMessage"] = $"You borrowed \"{book.Title}\" successfully! Total cost: {loan.BorrowCost:C}";
            return RedirectToAction("Index", "Books");
        }
        [Authorize(Roles = "Member")]
        [HttpGet]
        public async Task<IActionResult> MyLoans()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var loans = await _loanRepository.GetActiveLoansForMemberAsync(member.Id);
            return View(loans);
        }

        [Authorize(Roles = "Member")]
        [HttpGet]
        public async Task<IActionResult> LoanHistory()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var loans = await _loanRepository.GetLoanHistoryForMemberAsync(member.Id);
            return View(loans);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Return(int loanId)
        {
            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            var loan = await _loanRepository.GetByIdWithBookAsync(loanId);

            if (loan == null)
            {
                return NotFound();
            }

            if (loan.MemberId != member.Id)
            {
                return Forbid();
            }

            if (loan.ReturnDate.HasValue)
            {
                return RedirectToAction("MyLoans");
            }

            loan.ReturnDate = DateTime.Now;
            loan.Fine = FineCalculator.Calculate(loan.DueDate, loan.ReturnDate.Value);

            if (loan.Book != null)
            {
                loan.Book.AvailableCopies += 1;
                _bookRepository.Update(loan.Book);

                // لو فيه حد مستني الكتاب ده، بلّغه إنه بقى جاهز
                var nextInLine = await _reservationRepository.GetOldestWaitingForBookAsync(loan.Book.Id);
                if (nextInLine != null)
                {
                    nextInLine.Status = ReservationStatus.Ready;
                    nextInLine.ReadyExpiresAt = DateTime.Now.AddHours(48);
                }
            }

            await _loanRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Loan {LoanId} returned by Member {MemberId}. Fine: {Fine}",
                loan.Id, member.Id, loan.Fine ?? 0);

            if (loan.Fine.HasValue && loan.Fine > 0)
            {
                TempData["ErrorMessage"] = $"Book returned late. Fine: {loan.Fine:C}";
            }
            else
            {
                TempData["SuccessMessage"] = "Book returned successfully!";
            }

            return RedirectToAction("MyLoans");
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> ActiveLoans()
        {
            var loans = await _loanRepository.GetAllActiveLoansAsync();
            return View(loans);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> UnpaidFines()
        {
            var loans = await _loanRepository.GetUnpaidFinesAsync();
            return View(loans);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> MarkFinePaid(int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);

            if (loan == null)
            {
                return NotFound();
            }

            loan.IsFinePaid = true;
            await _loanRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Fine marked as paid.";
            return RedirectToAction("UnpaidFines");
        }
        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Renew(int loanId, int extraDays)
        {
            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            var loan = await _loanRepository.GetByIdAsync(loanId);

            if (loan == null)
            {
                return NotFound();
            }

            if (loan.MemberId != member.Id)
            {
                return Forbid();
            }

            if (loan.ReturnDate.HasValue)
            {
                TempData["ErrorMessage"] = "This book has already been returned.";
                return RedirectToAction("MyLoans");
            }

            if (loan.IsRenewed)
            {
                TempData["ErrorMessage"] = "This loan has already been renewed once.";
                return RedirectToAction("MyLoans");
            }

            if (loan.DueDate.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "You cannot renew an overdue loan. Please return the book.";
                return RedirectToAction("MyLoans");
            }

            if (extraDays < 1 || extraDays > 7)
            {
                TempData["ErrorMessage"] = "Renewal duration must be between 1 and 7 days.";
                return RedirectToAction("MyLoans");
            }

            loan.DueDate = loan.DueDate.AddDays(extraDays);
            loan.IsRenewed = true;
            loan.BorrowCost += loan.Book!.BorrowPrice * extraDays;

            await _loanRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Loan renewed successfully! New due date: {loan.DueDate:d MMM yyyy}";
            return RedirectToAction("MyLoans");
        }
        private async Task<Member?> GetCurrentMemberAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (identityUserId == null)
            {
                return null;
            }

            return await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
        }
    }
}