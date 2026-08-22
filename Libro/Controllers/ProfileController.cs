using Libro.Repositories;
using Libro.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMemberRepository _memberRepository;
        private readonly ILibrarianRepository _librarianRepository;
        private readonly ILoanRepository _loanRepository;

        public ProfileController(
            UserManager<IdentityUser> userManager,
            IMemberRepository memberRepository,
            ILibrarianRepository librarianRepository,
            ILoanRepository loanRepository)
        {
            _userManager = userManager;
            _memberRepository = memberRepository;
            _librarianRepository = librarianRepository;
            _loanRepository = loanRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (identityUserId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(identityUserId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Email = user.Email
            };

            if (User.IsInRole("Member"))
            {
                var member = await _memberRepository.GetByIdentityUserIdAsync(identityUserId);

                if (member == null)
                {
                    return NotFound();
                }

                model.FullName = member.FullName;
                model.ImageUrl = member.ImageUrl;
                model.Role = "Member";
                model.ActiveLoansCount = await _loanRepository.CountActiveLoansForMemberAsync(member.Id);
                model.TotalFines = await _loanRepository.SumFinesForMemberAsync(member.Id);
            }
            else if (User.IsInRole("Librarian"))
            {
                var librarian = await _librarianRepository.GetByIdentityUserIdAsync(identityUserId);

                if (librarian == null)
                {
                    return NotFound();
                }

                model.FullName = librarian.FullName;
                model.ImageUrl = librarian.ImageUrl;
                model.Role = "Librarian";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? currentName = null;
            string? currentImageUrl = null;

            if (identityUserId != null)
            {
                if (User.IsInRole("Member"))
                {
                    var member = await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
                    currentName = member?.FullName;
                    currentImageUrl = member?.ImageUrl;
                }
                else if (User.IsInRole("Librarian"))
                {
                    var librarian = await _librarianRepository.GetByIdentityUserIdAsync(identityUserId);
                    currentName = librarian?.FullName;
                    currentImageUrl = librarian?.ImageUrl;
                }
            }

            ViewBag.CurrentName = currentName;
            ViewBag.CurrentImageUrl = currentImageUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string fullName, string? imageUrl)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (identityUserId == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Member"))
            {
                var member = await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
                if (member != null)
                {
                    member.FullName = fullName;
                    member.ImageUrl = imageUrl;
                    await _memberRepository.SaveChangesAsync();
                }
            }
            else if (User.IsInRole("Librarian"))
            {
                var librarian = await _librarianRepository.GetByIdentityUserIdAsync(identityUserId);
                if (librarian != null)
                {
                    librarian.FullName = fullName;
                    librarian.ImageUrl = imageUrl;
                    await _librarianRepository.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}