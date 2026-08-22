using Libro.Models;
using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorRepository.GetByIdWithBooksAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Index()
        {
            var authors = await _authorRepository.GetAllAsync();
            return View(authors);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View(author);
            }

            await _authorRepository.AddAsync(author);
            await _authorRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author added successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorRepository.GetByIdWithBooksAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            _authorRepository.Update(author);
            await _authorRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author updated successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorRepository.GetByIdWithBooksAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _authorRepository.GetByIdWithBooksAsync(id);
            if (author == null) return NotFound();

            if (author.Books != null && author.Books.Any())
            {
                ModelState.AddModelError("", "Cannot delete an author who has books in the system.");
                return View("Delete", author);
            }

            _authorRepository.Remove(author);
            await _authorRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}