using Libro.Models;
using Libro.Repositories;
using Libro.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Libro.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IReviewRepository _reviewRepository;

        private const string RecentlyViewedSessionKey = "RecentlyViewedBookIds";
        private const int MaxRecentlyViewed = 5;
        private const int PageSize = 5;

        public BooksController(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IMemberRepository memberRepository,
            ILoanRepository loanRepository,
            ICategoryRepository categoryRepository,
            IFavoriteRepository favoriteRepository,
            IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
            _categoryRepository = categoryRepository;
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
        }

        // GET: BooksController
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            int totalBooks = await _bookRepository.CountAsync(searchTerm);
            int totalPages = (int)Math.Ceiling(totalBooks / (double)PageSize);

            var books = await _bookRepository.GetAllAsync(searchTerm, page, PageSize);

            var model = new BookListViewModel
            {
                Books = books,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages
            };

            if (User.IsInRole("Member"))
            {
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (identityUserId != null)
                {
                    var member = await _memberRepository.GetByIdentityUserIdAsync(identityUserId);

                    if (member != null)
                    {
                        ViewBag.BorrowedBookIds = await _loanRepository.GetActiveBookIdsForMemberAsync(member.Id);
                        ViewBag.ActiveLoansCount = await _loanRepository.CountActiveLoansForMemberAsync(member.Id);
                        ViewBag.MaxLoansReached = ViewBag.ActiveLoansCount >= 3;
                        ViewBag.FavoriteBookIds = await _favoriteRepository.GetFavoriteBookIdsAsync(member.Id);
                    }
                }
            }
            var recentRaw = HttpContext.Session.GetString(RecentlyViewedSessionKey);

            if (!string.IsNullOrEmpty(recentRaw))
            {
                var recentIds = recentRaw.Split(',').Select(int.Parse).ToList();
                var recentBooks = await _bookRepository.GetByIdsAsync(recentIds);

                // .Where + .Contains بترجع الترتيب مش بالضرورة زي الـ IDs، فبنعيد الترتيب يدوي
                ViewBag.RecentlyViewedBooks = recentIds
                    .Select(id => recentBooks.FirstOrDefault(b => b.Id == id))
                    .Where(b => b != null)
                    .ToList();
            }
            return View(model);
        }



        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetByIdWithAuthorAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            TrackRecentlyViewed(id);

            ViewBag.Reviews = await _reviewRepository.GetReviewsForBookAsync(id);
            ViewBag.AverageRating = await _reviewRepository.GetAverageRatingAsync(id);
            ViewBag.ReviewCount = await _reviewRepository.GetReviewCountAsync(id);

            if (User.IsInRole("Member"))
            {
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (identityUserId != null)
                {
                    var member = await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
                    if (member != null)
                    {
                        ViewBag.IsFavorite = await _favoriteRepository.IsFavoriteAsync(id, member.Id);
                        ViewBag.CanReview = await _loanRepository.HasMemberCompletedLoanAsync(id, member.Id);
                        ViewBag.MyReview = await _reviewRepository.GetMemberReviewForBookAsync(id, member.Id);
                    }
                }
            }

            return View(book);
        }

        private void TrackRecentlyViewed(int bookId)
        {
            var raw = HttpContext.Session.GetString(RecentlyViewedSessionKey);
            var ids = string.IsNullOrEmpty(raw)
                ? new List<int>()
                : raw.Split(',').Select(int.Parse).ToList();

            ids.Remove(bookId);
            ids.Insert(0, bookId);

            if (ids.Count > MaxRecentlyViewed)
            {
                ids = ids.Take(MaxRecentlyViewed).ToList();
            }

            HttpContext.Session.SetString(RecentlyViewedSessionKey, string.Join(",", ids));
        }
        

        // GET: BooksController/Create
        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var authors = await _authorRepository.GetAllAsync();
            ViewBag.Authors = new SelectList(authors, "Id", "Name");
            ViewBag.StatusOptions = GetStatusOptions();
            ViewBag.Categories = await _categoryRepository.GetAllAsync();   // ← ده لازم يكون موجود

            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> Create(Book book, int[] selectedCategoryIds)
        {
            if (!ModelState.IsValid)
            {
                var authors = await _authorRepository.GetAllAsync();
                ViewBag.Authors = new SelectList(authors, "Id", "Name");
                ViewBag.StatusOptions = GetStatusOptions();
                ViewBag.Categories = await _categoryRepository.GetAllAsync();
                return View(book);
            }

            book.AvailableCopies = book.TotalCopies;
            book.Categories = new List<Category>();

            foreach (var categoryId in selectedCategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null)
                {
                    book.Categories.Add(category);
                }
            }

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book added successfully!";
            return RedirectToAction("Index");
        }

        // GET: BooksController/Edit/5
        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var authors = await _authorRepository.GetAllAsync();
            ViewBag.Authors = new SelectList(authors, "Id", "Name");
            ViewBag.StatusOptions = GetStatusOptions();

            return View(book);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            var existingBook = await _bookRepository.GetByIdNoTrackingAsync(id);

            if (existingBook == null)
            {
                return NotFound();
            }

            if (await _bookRepository.IsIsbnTakenAsync(book.Isbn, id))
            {
                ModelState.AddModelError(nameof(Book.Isbn), "This ISBN is already used by another book.");
            }

            if (!ModelState.IsValid)
            {
                var authors = await _authorRepository.GetAllAsync();
                ViewBag.Authors = new SelectList(authors, "Id", "Name");
                ViewBag.StatusOptions = GetStatusOptions();
                return View(book);
            }

            int copiesDifference = book.TotalCopies - existingBook.TotalCopies;
            book.AvailableCopies = existingBook.AvailableCopies + copiesDifference;

            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book updated successfully!";
            return RedirectToAction("Index");
        }

        // GET: BooksController/Delete/5
        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdWithAuthorAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            bool hasActiveLoans = await _bookRepository.HasActiveLoansAsync(id);

            if (hasActiveLoans)
            {
                TempData["ErrorMessage"] = "Cannot delete this book — it has active loans.";
                return RedirectToAction("Index");
            }

            _bookRepository.Remove(book);
            await _bookRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book deleted successfully!";
            return RedirectToAction("Index");
        }

        // ---------------------------------------------------
        // Helper: نفس قايمة الـ Status اتكررت 4 مرات، بقت هنا مرة واحدة
        // ---------------------------------------------------
        private List<SelectListItem> GetStatusOptions()
        {
            return Enum.GetValues<BookStatus>()
                .Where(s => s != BookStatus.Borrowed)
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                })
                .ToList();
        }
    }
}