using Libro.Models;
using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Controllers
{
    [Authorize(Roles = "Librarian")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category added successfully!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdWithBooksAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdWithBooksAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Books != null && category.Books.Any())
            {
                ModelState.AddModelError("", "Cannot delete a category that has books assigned to it.");
                return View("Delete", category);
            }

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}