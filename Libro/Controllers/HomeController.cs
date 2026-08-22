using Libro.Data;
using Libro.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                TotalTitles = _context.Books.Count(),
                TotalCopiesAvailable = _context.Books.Sum(b => (int?)b.AvailableCopies) ?? 0,
                TotalMembers = _context.Members.Count()
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (User.IsInRole("Member"))
                {
                    var member = _context.Members.FirstOrDefault(m => m.IdentityUserId == identityUserId);
                    ViewBag.DisplayName = member?.FullName;
                }
                else if (User.IsInRole("Librarian"))
                {
                    var librarian = _context.Librarians.FirstOrDefault(l => l.IdentityUserId == identityUserId);
                    ViewBag.DisplayName = librarian?.FullName;
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
