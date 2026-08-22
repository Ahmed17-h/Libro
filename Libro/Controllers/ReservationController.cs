using Libro.Models;
using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    [Authorize(Roles = "Member")]
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;

        public ReservationController(
            IReservationRepository reservationRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository)
        {
            _reservationRepository = reservationRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            if (book.CanBeBorrowed)
            {
                TempData["ErrorMessage"] = "This book is currently available — you can borrow it directly.";
                return RedirectToAction("Index", "Books");
            }

            var existing = await _reservationRepository.GetActiveReservationAsync(bookId, member.Id);

            if (existing != null)
            {
                TempData["ErrorMessage"] = "You already have an active reservation for this book.";
                return RedirectToAction("Index", "Books");
            }

            var reservation = new Reservation
            {
                BookId = bookId,
                MemberId = member.Id,
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Waiting
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = $"You've been added to the waiting list for \"{book.Title}\".";
            return RedirectToAction("Index", "Books");
        }

        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var reservations = await _reservationRepository.GetMyReservationsAsync(member.Id);
            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int reservationId)
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.MemberId != member.Id)
            {
                return Forbid();
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _reservationRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation cancelled.";
            return RedirectToAction("MyReservations");
        }

        private async Task<Member?> GetCurrentMemberAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (identityUserId == null) return null;
            return await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
        }
    }
}